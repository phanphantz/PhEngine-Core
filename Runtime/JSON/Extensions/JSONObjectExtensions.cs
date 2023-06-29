using UnityEngine;
using System.Collections.Generic;
using System.Globalization;
using Debug = UnityEngine.Debug;

namespace PhEngine.Core.JSON
{
    public static class JSONObjectExtensions
    {
        public static JSONObject Create(params object[] args)
        {
            JSONObject json = new JSONObject();
            if (args.Length % 2 != 0)
            {
                Debug.LogError("JSONObject Error: JSONObject requires an even number of arguments!");
                return null;
            }
            else
            {
                int i = 0;
                while (i < args.Length - 1)
                {
                    object field = args[i];
                    object data = args[i + 1];
                    if (field.GetType() != typeof(string))
                    {
                        Debug.LogError("JSONObject Error: JSONObject requires field to be string!");
                        return null;
                    }

                    if (data.GetType() == typeof(string))
                        json.AddField(field.ToString(), (string) data);
                    else if (data.GetType() == typeof(JSONObject))
                        json.AddField(field.ToString(), (JSONObject) data);
                    else if (data.GetType() == typeof(int))
                        json.AddField(field.ToString(), (int) data);
                    else if (data.GetType() == typeof(float))
                        json.AddField(field.ToString(), (float) data);
                    else if (data.GetType() == typeof(bool))
                        json.AddField(field.ToString(), (bool) data);
                    else
                    {
                        Debug.LogError("JSONObject Error: JSONObject unsupport dataType! given type=" + data.GetType());
                        return null;
                    }

                    i += 2;
                }

                return json;
            }
        }

        #region Unity Transform,Vector3 Type Conversion

        //Vector 3
        public static string ToString_Compact(this Vector3 v)
        {
            string s = string.Format("{0},{1},{2}",
                v.x.ToString("F2"),
                v.y.ToString("F2"),
                v.z.ToString("F2"));
            s = s.Replace(".00", "");
            return s;
        }

        public static JSONObject ToJSON(this Vector3 v)
        {
            JSONObject json = new JSONObject();
            json.AddField("x", v.x);
            json.AddField("y", v.y);
            json.AddField("z", v.z);
            return json;
        }


        public static Vector3 ToVector3(this string s)
        {
            string[] list = s.Split(',');
            if (list.Length != 3)
            {
                Debug.LogWarning("string.ToVector3() , incorrect string format. return Vector3.Zero.");
                return Vector3.zero;
            }

            //Debug.Log(list.ToStringPretty("#"));
            Vector3 v = new Vector3(
                float.Parse(list[0], CultureInfo.InvariantCulture),
                float.Parse(list[1], CultureInfo.InvariantCulture),
                float.Parse(list[2], CultureInfo.InvariantCulture)
            );
            return v;
        }


        public static Vector3 ToVector3(this JSONObject j)
        {
            if (j.IsVector3())
            {
                Vector3 v = new Vector3(
                    j.SafeFloat("x", 0),
                    j.SafeFloat("y", 0),
                    j.SafeFloat("z", 0)
                );
                return v;
            }
            else
            {
                return Vector3.zero;
            }
        }

        public static bool IsVector3(this JSONObject j)
        {
            if (
                j.Count == 3 &&
                j.HasField("x") &&
                j.HasField("y") &&
                j.HasField("z") &&
                j["x"].IsNumber &&
                j["y"].IsNumber &&
                j["z"].IsNumber
            )
            {
                return true;
            }

            return false;
        }


        //Transform
        private enum TransformMaskIndex
        {
            Position = 0,
            Scale = 1,
            Rotation = 2,
            IsCompact = 3,
        };

        [System.Flags]
        public enum TransformMask
        {
            Position = 1 << (int) TransformMaskIndex.Position,
            Scale = 1 << (int) TransformMaskIndex.Scale,
            Rotation = 1 << (int) TransformMaskIndex.Rotation,
            Compact = 1 << (int) TransformMaskIndex.IsCompact,

            Position_Scale = 1 << (int) TransformMaskIndex.Position | 1 << (int) TransformMaskIndex.Scale,
            Position_Rotation = 1 << (int) TransformMaskIndex.Position | 1 << (int) TransformMaskIndex.Rotation,
            Scale_Rotation = 1 << (int) TransformMaskIndex.Scale | 1 << (int) TransformMaskIndex.Rotation,

            All =
                1 << (int) TransformMaskIndex.Position |
                1 << (int) TransformMaskIndex.Scale |
                1 << (int) TransformMaskIndex.Rotation,
        }

        public static int CreateTransformMask(bool position, bool scale, bool rotation, bool compact)
        {
            int mask = 0;
            if (position)
                FlagsHelper.Set(ref mask, (int) TransformMask.Position);
            if (scale)
                FlagsHelper.Set(ref mask, (int) TransformMask.Scale);
            if (rotation)
                FlagsHelper.Set(ref mask, (int) TransformMask.Rotation);
            if (compact)
                FlagsHelper.Set(ref mask, (int) TransformMask.Compact);


            return mask;
        }

        public static string ToJSON_CustomString(this Transform t)
        {
            return string.Format("#!p!:{0},!r!:{1}$", t.localPosition.ToString_Compact(),
                t.localRotation.eulerAngles.ToString_Compact());
        }

        public static JSONObject ToJSON(this Transform t, int mask = (int) TransformMask.All)
        {
            JSONObject json = new JSONObject();


            if (FlagsHelper.IsSet(mask, (int) TransformMask.Compact))
            {
                if (FlagsHelper.IsSet(mask, (int) TransformMask.Position))
                    json.AddField("p", t.localPosition.ToString_Compact());
                if (FlagsHelper.IsSet(mask, (int) TransformMask.Scale))
                    json.AddField("s", t.localScale.ToString_Compact());
                if (FlagsHelper.IsSet(mask, (int) TransformMask.Rotation))
                    json.AddField("r", t.localRotation.eulerAngles.ToString_Compact());
            }
            else
            {
                if (FlagsHelper.IsSet(mask, (int) TransformMask.Position))
                    json.AddField("localPosition", t.localPosition.ToJSON());
                if (FlagsHelper.IsSet(mask, (int) TransformMask.Scale))
                    json.AddField("localScale", t.localScale.ToJSON());
                if (FlagsHelper.IsSet(mask, (int) TransformMask.Rotation))
                    json.AddField("localRotation", t.localRotation.eulerAngles.ToJSON());
            }


            return json;
        }

        public static void FromJSON(this Transform t, JSONObject j, int mask = (int) TransformMask.All)
        {
            if (j.IsTransform(mask))
            {
                if (FlagsHelper.IsSet(mask, (int) TransformMask.Compact))
                {
                    if (FlagsHelper.IsSet(mask, (int) TransformMask.Position))
                        t.localPosition = j.SafeString("p").ToVector3();
                    if (FlagsHelper.IsSet(mask, (int) TransformMask.Scale))
                        t.localScale = j.SafeString("s").ToVector3();
                    if (FlagsHelper.IsSet(mask, (int) TransformMask.Rotation))
                        t.localRotation = Quaternion.Euler(j.SafeString("r").ToVector3());
                }
                else
                {
                    if (FlagsHelper.IsSet(mask, (int) TransformMask.Position))
                        t.localPosition = j["localPosition"].ToVector3();
                    if (FlagsHelper.IsSet(mask, (int) TransformMask.Scale))
                        t.localScale = j["localScale"].ToVector3();
                    if (FlagsHelper.IsSet(mask, (int) TransformMask.Rotation))
                        t.localRotation = Quaternion.Euler(j["localRotation"].ToVector3());
                }
            }
            else
            {
                Debug.LogWarning("Transform.FromJSON() , incorrect json format. Transform won't update.");
            }
        }

        public static bool IsTransform(this JSONObject j, int mask = (int) TransformMask.All)
        {
            if (FlagsHelper.IsSet(mask, (int) TransformMask.Compact))
            {
                if (FlagsHelper.IsSet(mask, (int) TransformMask.Position))
                {
                    const string key = "p";
                    if ((j.HasField(key) && j[key].IsString) == false)
                        return false;
                }

                if (FlagsHelper.IsSet(mask, (int) TransformMask.Rotation))
                {
                    const string key = "r";
                    if ((j.HasField(key) && j[key].IsString) == false)
                        return false;
                }

                if (FlagsHelper.IsSet(mask, (int) TransformMask.Scale))
                {
                    const string key = "s";
                    if ((j.HasField(key) && j[key].IsString) == false)
                        return false;
                }
            }
            else
            {
                if (FlagsHelper.IsSet(mask, (int) TransformMask.Position))
                {
                    const string key = "localPosition";
                    if ((j.HasField(key) && j[key].IsVector3()) == false)
                        return false;
                }

                if (FlagsHelper.IsSet(mask, (int) TransformMask.Rotation))
                {
                    const string key = "localRotation";
                    if ((j.HasField(key) && j[key].IsVector3()) == false)
                        return false;
                }

                if (FlagsHelper.IsSet(mask, (int) TransformMask.Scale))
                {
                    const string key = "localScale";
                    if ((j.HasField(key) && j[key].IsVector3()) == false)
                        return false;
                }
            }


            return true;
        }

        public static JSONObject ToJSON(this Quaternion v)
        {
            return v.eulerAngles.ToJSON();
        }

        #endregion

        #region JSON to Array

        public static JSONObject ToJSONArray(this string[] arr)
        {
            JSONObject json = new JSONObject(JSONObject.Type.ARRAY);
            foreach (string s in arr)
                json.Add(s);

            return json;
        }

        public static JSONObject ToJSONArray(this int[] arr)
        {
            JSONObject json = new JSONObject(JSONObject.Type.ARRAY);
            foreach (int s in arr)
                json.Add(s);

            return json;
        }

        public static JSONObject ToJSONArray(this float[] arr)
        {
            JSONObject json = new JSONObject(JSONObject.Type.ARRAY);
            foreach (float s in arr)
                json.Add(s);

            return json;
        }

        public static string[] ToStringArray(this JSONObject jObj)
        {
            if (jObj.type != JSONObject.Type.ARRAY)
            {
                Debug.LogWarning("JSONObject:ToStringArray this is not array type , return");
                return null;
            }

            List<string> list = new List<string>();
            for (int i = 0; i < jObj.Count; i++)
            {
                if (jObj[i].IsString == false)
                    continue;

                list.Add(jObj[i].str);
            }

            return list.ToArray();
        }

        public static int[] ToIntArray(this JSONObject jObj)
        {
            if (jObj.type != JSONObject.Type.ARRAY)
            {
                Debug.LogWarning("JSONObject:ToStringArray this is not array type , return");
                return null;
            }

            List<int> list = new List<int>();
            for (int i = 0; i < jObj.Count; i++)
            {
                if (jObj[i].IsNumber == false)
                    continue;

                list.Add((int) jObj[i].n);
            }

            return list.ToArray();
        }

        public static float[] ToFloatArray(this JSONObject jObj)
        {
            if (jObj.type != JSONObject.Type.ARRAY)
            {
                Debug.LogWarning("JSONObject:ToStringArray this is not array type , return");
                return null;
            }

            List<float> list = new List<float>();
            for (int i = 0; i < jObj.Count; i++)
            {
                if (jObj[i].IsNumber == false)
                    continue;

                list.Add((float) jObj[i].n);
            }

            return list.ToArray();
        }

        #endregion

        #region Add/Remove Json from Json.Type.Array

        /// <summary>
        /// return first object with given key and matched value
        /// </summary>
        /// <returns>The object from array.</returns>
        /// <param name="jObj">J object.</param>
        /// <param name="key">Key.</param>
        /// <param name="value">Value.</param>
        public static JSONObject GetObjectFromArray(this JSONObject jObj, string key, string value)
        {
            if (jObj.type != JSONObject.Type.ARRAY)
            {
                Debug.LogWarning("JSONObject:GetObject this is not array type , return");
                return null;
            }

            foreach (JSONObject j in jObj.list)
            {
                if (j.HasField(key))
                {
                    string v = j.SafeString(key, "", false);
                    if (v == value)
                        return j;
                }
            }

            return null;
        }


        public static JSONObject GetObjectFromArray(this JSONObject jObj, string key, int value)
        {
            if (jObj.type != JSONObject.Type.ARRAY)
            {
                Debug.LogWarning("JSONObject:GetObject this is not array type , return");
                return null;
            }

            foreach (JSONObject j in jObj.list)
            {
                if (j.HasField(key))
                {
                    int v = j.SafeInt(key, -1, false);
                    if (v == value)
                        return j;
                }
            }

            return null;
        }

        public static JSONObject RemoveObjectFromArray(this JSONObject jObj, int index)
        {
            if (jObj.type != JSONObject.Type.ARRAY)
            {
                Debug.LogWarning("JSONObject:RemoveObj this is not array type , return");
                return null;
            }

            JSONObject removedObj = jObj.list[index];
            jObj.list.RemoveAt(index);

            return removedObj;
        }

        #endregion

        #region Safe getting value

        public static bool SafeBool(this JSONObject jObj, string index, bool defaultReturn = false,
            bool warningLog = false)
        {
            JSONObject targetField = jObj[index];
            if (targetField == null)
            {
                if (warningLog)
                    Debug.LogWarning(string.Format("JsonObject::SaftBool({0}) not found return default value:{1}",
                        index, defaultReturn.ToString()));
                return defaultReturn;
            }
            else if (targetField.IsBool == false)
            {
                if (warningLog)
                    Debug.LogWarning(string.Format("JsonObject::SaftBool({0}) not found return default value:{1}",
                        index, defaultReturn.ToString()));
                return defaultReturn;
            }

            return targetField.b;
        }

        public static string SafeString(this JSONObject jObj, string index, string defaultReturn = "",
            bool warningLog = false)
        {
            JSONObject targetField = jObj[index];
            if (targetField == null)
            {
                if (warningLog)
                    Debug.LogWarning(string.Format("JsonObject::SaftString({0}) not found return default value:{1}",
                        index, defaultReturn));
                return defaultReturn;
            }
            else if (targetField.IsString)
            {
                return targetField.str;
            }
            else
            {
                if (targetField.IsNumber)
                {
                    return "" + targetField.n;
                }
                else
                {
                    if (warningLog)
                        Debug.LogWarning(string.Format("JsonObject::SaftString({0}) not found return default value:{1}",
                            index, defaultReturn));
                    return defaultReturn;
                }
            }
        }


        public static float SafeFloat(this JSONObject jObj, string index, float defaultReturn = 0.0f,
            bool warningLog = false)
        {
            JSONObject targetField = jObj[index];
            if (targetField == null)
            {
                if (warningLog)
                    Debug.LogWarning(string.Format("JsonObject::SaftFloat({0}) not found return default value:{1}",
                        index, defaultReturn));
                return defaultReturn;
            }
            else if (targetField.IsNumber == false)
            {
                if (targetField.IsString)
                {
                    float result = 0;
                    if (float.TryParse(targetField.str, out result))
                        return (float) result;
                }

                if (warningLog)
                    Debug.LogWarning(string.Format("JsonObject::SaftFloat({0}) not found return default value:{1}",
                        index, defaultReturn));
                return defaultReturn;
            }


            return (float) targetField.n;
        }

        public static int SafeInt(this JSONObject jObj, string index, int defaultReturn = 0, bool warningLog = false)
        {
            JSONObject targetField = jObj[index];
            if (targetField == null)
            {
                if (warningLog)
                    Debug.LogWarning(string.Format("JsonObject::SaftInt({0}) not found return default value:{1}", index,
                        defaultReturn));
                return defaultReturn;
            }

            var rawString = targetField.ToString();
            var isShouldUseDouble = rawString.Length >= 6;
            if (isShouldUseDouble)
            {
                if (double.TryParse(rawString, out var doubleValue))
                    return (int) doubleValue;
            }
            
            if (targetField.IsNumber == false)
            {
                if (targetField.IsString)
                {
                    int result = 0;
                    if (int.TryParse(targetField.str, out result))
                        return (int) result;
                }

                if (warningLog)
                    Debug.LogWarning(string.Format("JsonObject::SaftInt({0}) not found return default value:{1}", index,
                        defaultReturn));
                return defaultReturn;
            }

            return Mathf.RoundToInt(targetField.n);
        }

        #endregion

        public static JSONObject ToJSONArray(this JSONObject[] jObjs)
        {
            JSONObject arr = new JSONObject(JSONObject.Type.ARRAY);
            foreach (JSONObject j in jObjs)
            {
                arr.Add(j);
            }

            return arr;
        }

        public static void SetField(this JSONObject jObj, string name, double val)
        {
            jObj.SetField(name, JSONObject.Create((float) val));
        }
    }
}