using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using UnityEngine;

namespace PhEngine.Core.JSON
{
    public static class JSONConverter
    {
        public static JSONObject From(JSONConvertibleObject convertibleObject)
        {
            var propertyDictionary = JSONDictionaryUtils.CreateValueDictionary(convertibleObject);
            var resultJson = From(propertyDictionary);
            return resultJson;
        }
        
        static JSONObject From(Dictionary<string, object> dictionary)
        {
            var resultJson = new JSONObject();
            InjectDictionary(dictionary, ref resultJson);
            return resultJson;
        }

        static void InjectDictionary(Dictionary<string, object> dictionary, ref JSONObject resultJson)
        {
            foreach (var prop in dictionary)
                Inject(prop.Value, ref resultJson, prop.Key);
        }

        static void Inject(object obj, ref JSONObject resultJson, string fieldName = null)
        {
            switch (obj)
            {
                //Handle nested JSON Convertible Object
                case JSONConvertibleObject tryGetJsonConvertibleObject:
                {
                    var convertibleObjectJson = tryGetJsonConvertibleObject.CreateJSON();
                    if (fieldName == null)
                        resultJson.Add(convertibleObjectJson);
                    else
                        resultJson.AddField(fieldName, convertibleObjectJson);
                    
                    return;
                }

                //Handle basic types
                case int intValue when fieldName == null:
                    resultJson.Add(intValue);
                    break;
                case int intValue:
                    resultJson.AddField(fieldName, intValue);
                    break;
                case float floatValue when fieldName == null:
                    resultJson.Add(floatValue);
                    break;
                case float floatValue:
                    resultJson.AddField(fieldName, floatValue);
                    break;
                case string stringValue when fieldName == null:
                    resultJson.Add(stringValue);
                    break;
                case string stringValue:
                    resultJson.AddField(fieldName, stringValue);
                    break;
                case bool boolValue when fieldName == null:
                    resultJson.Add(boolValue);
                    break;
                case bool boolValue:
                    resultJson.AddField(fieldName, boolValue);
                    break;
                
                //Handle arrays
                case Array arrayObj:
                {
                    var arrayJson = GetArrayJson(arrayObj);
                    resultJson.AddField(fieldName, arrayJson);
                    break;
                }
            }

            JSONObject GetArrayJson(Array array)
            {
                var jsonObject = new JSONObject();
                foreach (var child in array)
                    Inject(child, ref jsonObject);

                return jsonObject;
            }
        }
        
        public static void SetValueByJSON(JSONConvertibleObject targetObject,JSONObject json)
        {
            if (json == null)
                throw new ArgumentNullException(nameof(json));
            
            var propertyValueList = JSONDictionaryUtils.CreateFieldDictionary(targetObject);
            foreach (var prop in propertyValueList)
                SetFieldValue(targetObject , json, prop);
        }

        static void SetFieldValue(JSONConvertibleObject targetObject, JSONObject json, KeyValuePair<FieldInfo, Type> prop)
        {
            var targetField = GetField(targetObject ,prop);
            SetField(targetObject ,targetField, json, prop);
        }

        static FieldInfo GetField(JSONConvertibleObject targetObject, KeyValuePair<FieldInfo, Type> prop)
        {
            return targetObject.GetType().GetField(prop.Key.Name);
        }

        static void SetField(JSONConvertibleObject targetObject, FieldInfo targetField, JSONObject json, KeyValuePair<FieldInfo, Type> prop)
        {
            var valueToSet = GetValueOfType(prop.Value, json, prop.Key);
            targetField.SetValue(targetObject, valueToSet);
        }

        static object GetValueOfType(Type objType, JSONObject resultJson, FieldInfo fieldInfo)
        {
            var fieldName = JSONFieldName.Find(fieldInfo);
            if (string.IsNullOrEmpty(fieldName))
                return null;

            //Handle JSON Convertible Object
            if (IsJSONConvertibleObject(objType))
            {
                if (resultJson[fieldName] != null)
                    return CreateObjectFromJSON(objType, resultJson[fieldName]);
                
                Debug.LogError("You pass in json that doesn't contain target fieldName to get value from. This might be caused by treating json elements as json list!");
                return null;
            }

            //Handle basic types
            if (objType == typeof(int))
                return resultJson.SafeInt(fieldName);

            if (objType == typeof(float))
                return resultJson.SafeFloat(fieldName);

            if (objType == typeof(string))
                return resultJson.SafeString(fieldName);

            if (objType == typeof(bool))
                return resultJson.SafeBool(fieldName);
            
            //Handle basic arrays
            if (!objType.IsArray)
                return null;

            if (resultJson[fieldName] == null)
                return null;
            
            var jsonList = resultJson[fieldName].list;
            if (jsonList == null)
                return null;
    
            var elementType = objType.GetElementType();
            if (elementType == typeof(int))
                return resultJson[fieldName].ToIntArray();
            if (elementType == typeof(string))
                return resultJson[fieldName].ToStringArray();
            if (elementType == typeof(float))
                return resultJson[fieldName].ToFloatArray();
            if (elementType == typeof(bool))
            {
                var resultStringArray = resultJson[fieldName].list;
                return GetBoolArrayFromJSONArray(resultStringArray);
            }

            //Handle JSON Convertible Object array
            if (!IsJSONConvertibleObjectArray(elementType))
                return null;

            var objectArray = jsonList
                .Select<JSONObject, object>(jsonObject => CreateObjectFromJSON(elementType, jsonObject)).ToArray();
           
            var resultArray = Array.CreateInstance(elementType, objectArray.Length);
            
            Array.Copy(objectArray, resultArray, objectArray.Length);
            return resultArray;
        }

        static bool IsJSONConvertibleObject(Type type)
        {
            return type.IsSubclassOf(typeof(JSONConvertibleObject));
        }

        static object GetBoolArrayFromJSONArray(List<JSONObject> resultStringArray)
        {
            return resultStringArray.Select(str => str.ToString() != "false").ToArray();
        }

        static bool IsJSONConvertibleObjectArray(Type elementType)
        {
            return elementType != null && IsJSONConvertibleObject(elementType);
        }

        public static object CreateObjectFromJSON(Type elementType, JSONObject jsonObject)
        {
            var instance = CreateInstanceOfType(elementType);
            SetValueByJSON(instance as JSONConvertibleObject , jsonObject);
            return instance;
        }

        public static object CreateInstanceOfType(Type type)
        {
            return FormatterServices.GetUninitializedObject(type);
        }
        
        public static List<T> ToList<T>(JSONObject json) where T : JSONConvertibleObject
        {
            var resultList = new List<T>();
            if (json == null || json.IsNull)
            {
                Debug.LogError("Cannot Get List. Entire JSON is null");
                return resultList;
            }

            //If json is not an array, we'll try to treat their elements as a whole single element.
            if (!json.IsArray)
            {
                CreateNewElementAndAddToList(json);
                return resultList;
            }

            //If json is an array, we'll try to treat their elements as list from now on.
            if (json.list == null)
            {
                Debug.LogError("Cannot Get List of " + nameof(T) + ". List from JSON is null");
                return resultList;
            }

            var jsonList = json.list;
            foreach (var jsonObject in jsonList)
                CreateNewElementAndAddToList(jsonObject);

            return resultList;

            void CreateNewElementAndAddToList(JSONObject json)
            {
                var newElement = To<T>(json);
                resultList.Add(newElement);
            }
        }

        public static T To<T>(JSONObject json) where T : JSONConvertibleObject
        {
            var instance = CreateInstanceOfType<T>();
            SetValueByJSON(instance,json);
            return instance;
        }

        public static T CreateInstanceOfType<T>() where T : JSONConvertibleObject
        {
            var type = typeof(T);
            return (T)FormatterServices.GetUninitializedObject(type);
        }

        public static void SetValueByJSON<T>(T instance, JSONObject json) where T : JSONConvertibleObject
        {
            instance.SetValueByJSON(json);
        }
    }
}