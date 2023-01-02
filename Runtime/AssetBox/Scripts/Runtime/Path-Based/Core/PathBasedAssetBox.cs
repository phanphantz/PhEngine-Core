using System.Collections;
using UnityEngine;

namespace PhEngine.Core.AssetBox
{
    public abstract partial class PathBasedAssetBox : AssetBoxGen<PathRef>, IPathBasedAssetBox
    {
        protected override Object LoadObjectFromReference<T>(PathRef pathRef)
        {
            var assetPath = pathRef.loadPath;
            if (string.IsNullOrEmpty(assetPath))
            {
                PhDebug.LogError<PathBasedAssetBox>(AssetBoxError.GetAssetNotFoundErrorMessage(pathRef.id, name));
                return default;
            }
            
            var assetObject = LoadObjectFromPath<T>(assetPath);
            if (assetObject != null) 
                return assetObject;
            
            PhDebug.LogError<PathBasedAssetBox>(AssetBoxError.GetAssetNotFoundErrorMessage(pathRef.id, name));
            return default;
        }
        
        protected abstract Object LoadObjectFromPath<T>(string path) where T : Object;

        protected override IEnumerator LoadObjectFromReferenceAsync<T1>(PathRef pathRef, AssetLoadRequestAsync<T1> request)
        {
            var assetPath = pathRef.loadPath;
            if (string.IsNullOrEmpty(assetPath))
            {
                PhDebug.LogError<PathBasedAssetBox>(AssetBoxError.GetAssetNotFoundErrorMessage(pathRef.id, name));
                yield break;
            }
            
            var loadOperation = GetLoadOperationByLoadMode(pathRef, request);
            while (!loadOperation.isDone)
            {
                NotifyProgress(request, loadOperation.progress);
                yield return null;
            }

            NotifyFinish(request, GetLoadedObjectByLoadMode(request, loadOperation));
        }
        
        AsyncOperation GetLoadOperationByLoadMode<T1>(PathRef pathRef, AssetLoadRequestAsync<T1> request) where T1 : Object
        {
            return request.Mode == AssetLoadMode.Asset
                ? GetLoadAsyncOperation<T1>(pathRef)
                : GetLoadAsyncOperation<GameObject>(pathRef);
        }

        Object GetLoadedObjectByLoadMode<T1>(AssetLoadRequestAsync<T1> request, AsyncOperation loadOperation) where T1 : Object
        {
            return request.Mode == AssetLoadMode.Asset
                ? GetLoadedObject<T1>(loadOperation)
                : GetLoadedObject<GameObject>(loadOperation);
        }
        
        protected abstract AsyncOperation GetLoadAsyncOperation<T>(PathRef pathRef) where T : Object;
        protected abstract T GetLoadedObject<T>(AsyncOperation loadOperation) where T : Object;
    }

}