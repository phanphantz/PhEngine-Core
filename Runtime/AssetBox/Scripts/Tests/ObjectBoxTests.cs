using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using System.Text.RegularExpressions;

namespace PhEngine.Core.AssetBox.Tests
{
    public class PrefabBoxTests
    {
        const string SAMPLE_GAME_OBJECT_ID = "sampleGameObject";
        const string SAMPLE_COMPONENT_ID = "sampleComponent";
        const string SAMPLE_TEXT_ASSET_ID = "sampleTextAsset";

        #region GameObject Loading

        [Test]
        public void test_load_game_object_pass()
        {
            var prefabBox = CreateSamplePrefabBoxWithGameObject();
            var loadedGameObject = prefabBox.LoadSceneObject<GameObject>(SAMPLE_GAME_OBJECT_ID);
            Assert.AreEqual(true, loadedGameObject != null);
        }

        [Test]
        public void test_load_fail_no_object_with_id()
        {
            var prefabBox = CreateSamplePrefabBoxWithGameObject();
            var wrongId = "wrong_id";
            prefabBox.LoadSceneObject<GameObject>(wrongId);
            LogAssert.Expect(LogType.Error, new Regex(AssetBoxError.GetRefNotFoundErrorMessage(wrongId, prefabBox.name)));
        }
        
        PrefabBox CreateSamplePrefabBoxWithGameObject()
        {
            var prefabBox = AssetBoxTestHelper.CreateTemporary<PrefabBox>();
            var gameObjectRef = CreateObjectRefOfGameObject(SAMPLE_GAME_OBJECT_ID, "GameObject");
            prefabBox.Pack(gameObjectRef);
            return prefabBox;
        }
        
        ObjectRef<GameObject> CreateObjectRefOfGameObject(string id, string gameObjectName)
        {
            var newGameObject = new GameObject
            {
                name = gameObjectName
            };
            return new ObjectRef<GameObject>(id, newGameObject);
        }

        #endregion

        #region Component Loading

        [Test]
        public void test_load_component_pass()
        {
            var prefabBox = CreateSamplePrefabBoxWithComponent();
            var loadedComponent = prefabBox.LoadSceneObject<SpriteRenderer>(SAMPLE_COMPONENT_ID);
            Assert.AreEqual(true, loadedComponent != null);
        }

        [Test]
        public void test_load_component_fail_type_mismatch()
        {
            var prefabBox = CreateSamplePrefabBoxWithComponent();
            prefabBox.LoadSceneObject<BoxCollider2D>(SAMPLE_COMPONENT_ID);
            LogAssert.Expect(LogType.Error,
                new Regex(AssetLoaderError.GetAssetTypeMismatchErrorMessage<BoxCollider2D>(SAMPLE_COMPONENT_ID)));
        }
        
        PrefabBox CreateSamplePrefabBoxWithComponent()
        {
            var objectDatabase = AssetBoxTestHelper.CreateTemporary<PrefabBox>();
            var componentRef = CreateObjectRefOfSampleComponent(SAMPLE_COMPONENT_ID, "GameObject");
            objectDatabase.Pack(componentRef);
            return objectDatabase;
        }

        ObjectRef<GameObject> CreateObjectRefOfSampleComponent(string id, string gameObjectName)
        {
            var newGameObject = new GameObject
            {
                name = gameObjectName
            };
            var component = newGameObject.AddComponent<SpriteRenderer>();
            return new ObjectRef<GameObject>(id, component.gameObject);
        }

        #endregion

        #region TextAsset Loading

        [Test]
        public void test_load_text_asset_pass()
        {
            var textAsset = CreateSampleTextAssetObjectDatabase();
            var loadedTextAsset = textAsset.LoadAsset<TextAsset>(SAMPLE_TEXT_ASSET_ID);
            Assert.AreEqual(true, loadedTextAsset != null);
        }

        [Test]
        public void test_load_text_asset_fail_type_mismatch()
        {
            var textAssetBox = CreateSampleTextAssetObjectDatabase();
            textAssetBox.LoadAsset<Sprite>(SAMPLE_TEXT_ASSET_ID);
            LogAssert.Expect(LogType.Error,
                new Regex(AssetLoaderError.GetAssetTypeMismatchErrorMessage<Sprite>(SAMPLE_TEXT_ASSET_ID)));
        }

        TextAssetBox CreateSampleTextAssetObjectDatabase()
        {
            var textAssetBox = AssetBoxTestHelper.CreateTemporary<TextAssetBox>();
            var textAssetRef = CreateObjectRefOfTextAsset(SAMPLE_TEXT_ASSET_ID, "TextAsset");
            textAssetBox.Pack(textAssetRef);
            return textAssetBox;
        }

        ObjectRef<TextAsset> CreateObjectRefOfTextAsset(string id, string textAssetName)
        {
            var newTextAsset = new TextAsset
            {
                name = textAssetName
            };
            return new ObjectRef<TextAsset>(id, newTextAsset);
        }

        #endregion
    }
    
}