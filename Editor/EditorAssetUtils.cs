using System.IO;
using UnityEditor;
using UnityEngine;

namespace PhEngine.Core.Editor
{
    public static class EditorAssetUtils
    {
        #region Path
        
        public static string CreateFullResourcePath(string path)
        {
            return Path.Combine(Application.dataPath, "Resources/" + path);
        }

        public static string GetFilename(Object obj)
        {
            return Path.GetFileName(AssetDatabase.GetAssetPath(obj));
        }

        #endregion
        
        #region Text File

        public static void CreateTextFileInResources(string pathWithFilename, string fileExtension ,string content)
        {
            CreateTextFile(CreateFullResourcePath(pathWithFilename), fileExtension, content);
        }
        
        public static void CreateTextFile(string pathWithFilename,string fileExtension, string content)
        {
            var fullPath = pathWithFilename + "." + fileExtension;
            File.WriteAllText(fullPath, content);
            AssetDatabase.Refresh();
        }

        #endregion

        #region Asset Deletion

        public static void DeleteAssetFromResources(string path)
        {
            DeleteAsset(CreateFullResourcePath(path));
        }
        
        public static void DeleteAsset(string path)
        {
            File.Delete(path);
            AssetDatabase.Refresh();
        }
        
        #endregion

        #region Prefab Creation

        public static GameObject CreatePrefabInResources(GameObject gameObjectToMakePrefab, string path)
        {
            return CreatePrefab(gameObjectToMakePrefab, CreateFullResourcePath(path));
        }

        public static GameObject CreatePrefab(GameObject gameObjectToMakePrefab, string path)
        {
            var prefab = PrefabUtility.SaveAsPrefabAsset(gameObjectToMakePrefab, path);
            AssetDatabase.Refresh();
            return prefab;
        }

        #endregion

        #region Scriptable Objects

        public static T[] FindAllScriptableObjects<T>() where T : ScriptableObject
        {
            var guids = AssetDatabase.FindAssets("t:" + typeof(T).Name);
            var results = new T[guids.Length];
            for (var i = 0; i < guids.Length; i++)
            {
                var path = AssetDatabase.GUIDToAssetPath(guids[i]);
                results[i] = AssetDatabase.LoadAssetAtPath<T>(path);
            }
            
            return results;
        }

        public static T CreateScriptableObject<T>(string filename, string path) where T : ScriptableObject
        {
            var asset = ScriptableObject.CreateInstance<T>();
            AssetDatabase.CreateAsset(asset, Path.Combine(path, filename + ".asset"));
            AssetDatabase.SaveAssets();

            return asset;
        }
        
        #endregion
    }
}