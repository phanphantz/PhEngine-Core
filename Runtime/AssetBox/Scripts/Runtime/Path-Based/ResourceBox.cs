using UnityEngine;

namespace PhEngine.Core.AssetBox
{
    [CreateAssetMenu(menuName = AssetBoxCreateAssetMenuPath.ASSET_BOX_PREFIX + nameof(ResourceBox), fileName = nameof(ResourceBox), order = 1)]
    public partial class ResourceBox : PathBasedAssetBox
    {
        protected override Object LoadObjectFromPath<T>(string path)
        {
            var tryLoadDirectly = Resources.Load<T>(path);
            return tryLoadDirectly ? tryLoadDirectly : Resources.Load(path);
        }

        protected override AsyncOperation GetLoadAsyncOperation<T>(PathRef pathRef)
            => Resources.LoadAsync<T>(pathRef.loadPath);

        protected override T GetLoadedObject<T>(AsyncOperation loadOperation)
            => (loadOperation as ResourceRequest)?.asset as T;
        
        public void Unload(Object targetObject)
            => Resources.UnloadAsset(targetObject);
        
        public AsyncOperation UnloadUnusedAssetsAsync()
            => Resources.UnloadUnusedAssets();
    }
}