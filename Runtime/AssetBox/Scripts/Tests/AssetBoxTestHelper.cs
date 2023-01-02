using PhEngine.Core.AssetBox.Editor;
using PhEngine.Core.Editor;
using UnityEngine;

namespace PhEngine.Core.AssetBox.Tests
{
    public static class AssetBoxTestHelper
    {
        const string COMPONENT_PREFAB_FILENAME = "component.prefab";
        const string GAME_OBJECT_PREFAB_FILENAME = "gameObject.prefab";
        const string TEXT_ASSET_FILENAME = "textAsset.txt";
        const string TEXT_ASSET_FILENAME_NO_EXTENSION = "textAsset";

        internal static T CreateTemporary<T>() where T : AssetBox
        {
            var database = ScriptableObject.CreateInstance<T>();
            return database;
        }

        internal static SpriteRenderer CreateSampleComponentPrefabInResources()
        {
            var newGameObject = new GameObject();
            var newComponent = newGameObject.AddComponent<SpriteRenderer>();
            var prefab =  EditorAssetUtils.CreatePrefabInResources(newComponent.gameObject, COMPONENT_PREFAB_FILENAME);
            return prefab.GetComponent<SpriteRenderer>();
        }

        internal static void DeleteSampleComponentPrefabFromResources()
        {
            EditorAssetUtils.DeleteAssetFromResources(COMPONENT_PREFAB_FILENAME);
        }
        
        internal static  GameObject CreateSampleGameObjectPrefabInResources()
        {
            var newGameObject = new GameObject();
            var prefab = EditorAssetUtils.CreatePrefabInResources(newGameObject, GAME_OBJECT_PREFAB_FILENAME);
            return prefab;
        }

        internal static void DeleteSamplePrefabFromResources()
        {
            EditorAssetUtils.DeleteAssetFromResources(GAME_OBJECT_PREFAB_FILENAME);
        }
        
        internal static void CreateSampleTextAssetInResources()
        {
            EditorAssetUtils.CreateTextFileInResources(TEXT_ASSET_FILENAME_NO_EXTENSION, "txt","sample");
        }

        internal static  void DeleteSampleTextAssetFromResources()
        {
            EditorAssetUtils.DeleteAssetFromResources(TEXT_ASSET_FILENAME);
        }

    }
}