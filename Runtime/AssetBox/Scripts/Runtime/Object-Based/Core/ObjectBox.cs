using System.Collections;
using UnityEngine;

namespace PhEngine.Core.AssetBox
{
    public abstract partial class ObjectBox<T> : AssetBoxGen<ObjectRef<T>>, IObjectBox
        where T : Object
    {
        protected override Object LoadObjectFromReference<T1>(ObjectRef<T> assetRef)
        {
            var assetObject = assetRef.targetObject;
            if (assetObject != null)
                return assetObject;

            PhDebug.LogError<ObjectBox<T>>(AssetBoxError.GetAssetNotFoundErrorMessage(assetRef.id, name));
            return default;
        }

        protected override IEnumerator LoadObjectFromReferenceAsync<T1>(ObjectRef<T> assetRef, AssetLoadRequestAsync<T1> request)
        {
            var assetObject = assetRef.targetObject;
            if (assetObject == null)
            {
                PhDebug.LogError<ObjectBox<T>>(AssetBoxError.GetAssetNotFoundErrorMessage(assetRef.id, name));
                yield break;
            }

            var targetObject = LoadObjectFromReference<T1>(assetRef);
            yield return null;
            NotifyProgress(request, 1f);
            NotifyFinish(request, targetObject);
        }
    }
}