using System.Collections;
using System.Text.RegularExpressions;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace PhEngine.Core.AssetBox.Tests
{
    public class ResourceBoxTests
    {
        const string TEXT_ASSET_ID = "sampleTextAsset";
        const string GAME_OBJECT_PREFAB_ID = "sampleGameObject";
        const string GAME_OBJECT_PREFAB_FILENAME_NO_EXTENSION = "gameObject";
        const string COMPONENT_PREFAB_ID = "sampleComponent";
        const string COMPONENT_PREFAB_FILENAME_NO_EXTENSION = "component";
        const string TEXT_ASSET_FILENAME_NO_EXTENSION = "textAsset";

        [UnityTest]
        public IEnumerator test_load_text_asset_pass()
        {
            //Setup
            AssetBoxTestHelper.CreateSampleTextAssetInResources();
            yield return null;
            var resourceBox = CreateSampleResourceBoxWithTextAsset();

            //Act
            var loadedTextAsset = resourceBox.LoadAsset<TextAsset>(TEXT_ASSET_ID);

            //Assert
            Assert.AreEqual(true, loadedTextAsset != null);
            
            //Clean up
            AssetBoxTestHelper.DeleteSampleTextAssetFromResources();
        }

        [UnityTest]
        public IEnumerator test_load_text_asset_fail_type_mismatch()
        {
            //Setup
            AssetBoxTestHelper.CreateSampleTextAssetInResources();
            yield return null;

            var resourceBox = CreateSampleResourceBoxWithTextAsset();

            //Act
            resourceBox.LoadAsset<Sprite>(TEXT_ASSET_ID);

            //Assert
            LogAssert.Expect(LogType.Error, new Regex(AssetLoaderError.GetAssetTypeMismatchErrorMessage<Sprite>(TEXT_ASSET_ID)));

            //Clean up
            AssetBoxTestHelper.DeleteSampleTextAssetFromResources();
        }
        
        static ResourceBox CreateSampleResourceBoxWithTextAsset()
        {
            var resourceBox = AssetBoxTestHelper.CreateTemporary<ResourceBox>();
            resourceBox.Pack( new PathRef(TEXT_ASSET_ID, TEXT_ASSET_FILENAME_NO_EXTENSION));
            return resourceBox;
        }
        
        [UnityTest]
        public IEnumerator test_load_game_object_pass()
        {
            //Setup
            var gameObjectPrefab = AssetBoxTestHelper.CreateSampleGameObjectPrefabInResources();
            yield return null;

            var resourceBox = CreateSampleResourceLoaderWithGameObject();

            //Act
            var loadedGameObject = resourceBox.LoadSceneObject<GameObject>(GAME_OBJECT_PREFAB_ID);

            //Assert
            Assert.AreEqual(true, loadedGameObject != null);
            Assert.AreEqual(false, gameObjectPrefab == loadedGameObject);

            //Clean up
            AssetBoxTestHelper.DeleteSamplePrefabFromResources();
        }

        ResourceBox CreateSampleResourceLoaderWithGameObject()
        {
            var resourceBox = AssetBoxTestHelper.CreateTemporary<ResourceBox>();
            resourceBox.Pack(new PathRef(GAME_OBJECT_PREFAB_ID, GAME_OBJECT_PREFAB_FILENAME_NO_EXTENSION));
            return resourceBox;
        }

        [UnityTest]
        public IEnumerator test_load_component_from_resources_instantiate_pass()
        {
            //Setup
            var componentPrefab = AssetBoxTestHelper.CreateSampleComponentPrefabInResources();
            yield return null;

            var resourceBox = CreateSampleResourceLoaderWithComponent();

            //Act
            var loadedComponent = resourceBox.LoadSceneObject<SpriteRenderer>(COMPONENT_PREFAB_ID);

            //Assert
            Assert.AreEqual(true, loadedComponent != null);
            Assert.AreEqual(false, componentPrefab == loadedComponent);

            //Clean up
            AssetBoxTestHelper.DeleteSampleComponentPrefabFromResources();
        }

        [UnityTest]
        public IEnumerator test_load_component_from_resources_fail_type_mismatch()
        {
            //Setup
            AssetBoxTestHelper.CreateSampleComponentPrefabInResources();
            yield return null;

            var resourceBox = CreateSampleResourceLoaderWithComponent();

            //Act
            resourceBox.LoadSceneObject<BoxCollider2D>(COMPONENT_PREFAB_ID);

            //Assert
            LogAssert.Expect(LogType.Error,
                new Regex(AssetLoaderError.GetAssetTypeMismatchErrorMessage<BoxCollider2D>(COMPONENT_PREFAB_ID)));

            //Clean up
            AssetBoxTestHelper.DeleteSampleComponentPrefabFromResources();
        }

        ResourceBox CreateSampleResourceLoaderWithComponent()
        {
            var resourceBox = AssetBoxTestHelper.CreateTemporary<ResourceBox>();
            resourceBox.Pack(new PathRef(COMPONENT_PREFAB_ID, COMPONENT_PREFAB_FILENAME_NO_EXTENSION));
            return resourceBox;
        }
        
    }
}