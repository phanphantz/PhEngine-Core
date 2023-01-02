using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace PhEngine.Core.AssetBox.Editor
{
    [CreateAssetMenu(menuName = AssetBoxCreateAssetMenuPath.ASSET_BOX_PREFIX + nameof(AssetBoxPacker), fileName = nameof(AssetBoxPacker))]
    public class AssetBoxPacker : ScriptableObject
    {
        [SerializeField] AssetBox target;
        [SerializeField] AssetType assetType;
        [SerializeField] DirectoryReference[] directoryPaths;
        
        [Header("Optional")]
        [SerializeField] AssetSearchRule searchRule;
        [SerializeField] AssetNameRule nameRule;
        
        public void PackBySelectedType()
        {
            switch (assetType)
            {
                case AssetType.Any:
                    Pack<Object>();
                    break;
                
                case AssetType.Prefab:
                    Pack<GameObject>();
                    break;
                
                case AssetType.Sprite:
                    Pack<Sprite>();
                    break;
                
                case AssetType.TextAsset:
                    Pack<TextAsset>();
                    break;
                
                case AssetType.AudioClip:
                    Pack<AudioClip>();
                    break;
                
                case AssetType.Material:
                    Pack<Material>();
                    break;
                
                case AssetType.Texture:
                    Pack<Texture>();
                    break;
                
                case AssetType.Texture2D:
                    Pack<Texture2D>();
                    break;
                
                default:
                    throw new ArgumentOutOfRangeException(nameof(assetType), assetType, null);
            }
        }
        
        public void Pack<T>() where T : Object
        {
            var objectRefList = GetObjectRefList<T>();
            var castedAssetRef = objectRefList.Cast<AssetRef>().ToArray();
            target.Pack(castedAssetRef);
            EditorGUIUtility.PingObject(target);
        }

        List<ObjectRef<T>> GetObjectRefList<T>() where T : Object
        {
            var allObjectRefList = new List<ObjectRef<T>>();
            foreach (var directoryRef in directoryPaths)
            {
                var objectRefFromPathList = GetAllObjectRefFromPath<T>(directoryRef);
                allObjectRefList.AddRange(objectRefFromPathList);
            }

            return allObjectRefList.Where(assetRef => assetRef != null).ToList();
        }

        List<ObjectRef<T>> GetAllObjectRefFromPath<T>(DirectoryReference directoryRef) where T : Object
        {
            var targetObjectList = new List<ObjectRef<T>>();
            var fullPath = Path.Combine(Application.dataPath, directoryRef.path);
            var allFilesAtPath = Directory.GetFiles(fullPath, "*",  directoryRef.isIncludeSubFolders ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);
            foreach (var file in allFilesAtPath)
                PutFileIntoObjectRefList(file, targetObjectList);

            return targetObjectList;
        }

        void PutFileIntoObjectRefList<T>(string file, List<ObjectRef<T>> targetObjectList) where T : Object
        {
            var assetPath = "Assets" + file.Replace(Application.dataPath, "").Replace('\\', '/');
            var asset = AssetDatabase.LoadAssetAtPath<T>(assetPath);
            if (asset)
                PutObjectIntoObjectRefList(assetPath, asset, targetObjectList);
        }

        void PutObjectIntoObjectRefList<T>(string path, Object targetObject, List<ObjectRef<T>> objectRefList) where T : Object
        {
            var targetId = GetId(path, targetObject);
            var objectAsType = targetObject as T;
            if (objectAsType == null)
                return;
            
            var objectRef = CreateObjectRef(path, targetId, objectAsType);
            if (objectRef != null)
                objectRefList.Add(objectRef);
        }

        ObjectRef<T> CreateObjectRef<T>(string path, string targetId, T targetObject) where T : Object
        {
            var result = new ObjectRef<T>(targetId, targetObject);
            if (searchRule == null)
            {
                ReportPackedFile(targetId, path);
                return result;
            }

            if (searchRule.IsPass(path, targetObject))
            {
                ReportPackedFile(targetId, path);
                return result;
            }

            return null;
        }

        void ReportPackedFile(string id, string path)
        {
            Debug.Log($"Pack Asset Id: '{id}' from '{path}' into '{target.name}'");
        }

        string GetId<T>(string path, T targetObject) where T : Object
        {
            var targetName = targetObject.name;
            if (nameRule != null)
                targetName = nameRule.GetName(path, targetObject);
            
            return targetName;
        }
        
    }
}