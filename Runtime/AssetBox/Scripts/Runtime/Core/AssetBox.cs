using System;
using System.Collections;
using UnityEngine;
using Object = UnityEngine.Object;

namespace PhEngine.Core.AssetBox
{
    [Serializable]
    public abstract partial class AssetBox : ScriptableObject
    {
        #region Normal Load
        
        public T LoadSceneObject<T>(string id, Transform parent = null) where T : Object
            => GetTargetAndLoad<T>(new AssetLoadRequest(id, AssetLoadMode.SceneObject, parent));
        
        public T LoadUI<T>(string id, Transform parent = null) where T : Object
            => GetTargetAndLoad<T>(new AssetLoadRequest(id, AssetLoadMode.UI, parent));

        public T LoadAsset<T>(string id) where T : Object
            => GetTargetAndLoad<T>(new AssetLoadRequest(id, AssetLoadMode.Asset, null));

        T GetTargetAndLoad<T>(AssetLoadRequest request) where T : Object
            => GetLoadTarget().Load<T>(request);
        
        #endregion

        #region Async Load

        public IEnumerator LoadSceneObjectAsync<T>(string id, Action<T> onFinish, Transform parent = null) where T : Object
            => LoadSceneObjectAsync(id, onFinish, null, parent);

        public IEnumerator LoadUIAsync<T>(string id, Action<T> onFinish, Transform parent = null) where T : Object
            => LoadUIAsync(id, onFinish, null, parent);

        public IEnumerator LoadSceneObjectAsync<T>(string id, Action<T> onFinish, Action<float> onProgress, Transform parent = null) where T : Object
            => GetTargetAndLoadAsync(new AssetLoadRequestAsync<T>(id, AssetLoadMode.SceneObject, parent, onFinish, onProgress));

        public IEnumerator LoadUIAsync<T>(string id, Action<T> onFinish, Action<float> onProgress ,Transform parent = null) where T : Object
            => GetTargetAndLoadAsync(new AssetLoadRequestAsync<T>(id, AssetLoadMode.UI, parent,  onFinish , onProgress));
        
        public IEnumerator LoadAssetAsync<T>(string id, Action<T> onFinish, Action<float> onProgress = null) where T : Object
            => GetTargetAndLoadAsync(new AssetLoadRequestAsync<T>(id, AssetLoadMode.Asset, null,  onFinish, onProgress));
        
        IEnumerator GetTargetAndLoadAsync<T>(AssetLoadRequestAsync<T> request) where T : Object
            => GetLoadTarget().LoadAsync(request);
        
        #endregion

        protected virtual AssetBox GetLoadTarget() => this;
        protected abstract T Load<T>(AssetLoadRequest request) where T : Object;
        protected abstract IEnumerator LoadAsync<T>(AssetLoadRequestAsync<T> request) where T : Object;
    }
    
}