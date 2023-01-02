using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

namespace PhEngine.Core.AssetBox
{
    public abstract partial class AssetBoxGen<T> : AssetBox
        where T : AssetRef
    {
        public IReadOnlyCollection<T> References => Array.AsReadOnly(references);
        [SerializeField] T[] references;
        
        public void Pack(params T[] refs)
        {
            references = refs;
#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(this);
#endif
        }
        
        T GetReferenceById(string id)
        {
            var result = references.FirstOrDefault(assetRef => assetRef.id == id);
            if (result != null)
                return result;
            
            PhDebug.LogError<AssetBoxGen<T>>(AssetBoxError.GetRefNotFoundErrorMessage(id, name));
            return null;
        }

        static T1 GetValidResult<T1>(AssetLoadRequest request, Object assetObject) where T1 : Object
        {
            var validator = new AssetLoadValidator<T1>(assetObject, request);
            var resultAsset = validator.GetValidObject();
            return resultAsset == null ?
                default : 
                resultAsset;
        }

        #region Normal Load
        
        protected override T1 Load<T1>(AssetLoadRequest request)
        {
            var assetRef = GetReferenceById(request.Id);
            return assetRef == null ? 
                default:
                LoadByReference<T1>(assetRef, request);
        }

        T1 LoadByReference<T1>(T assetRef ,AssetLoadRequest request) where T1 : Object
        {
            var assetObject = LoadObjectFromReference<T1>(assetRef);
            return assetObject == null ? 
                default : 
                GetValidResult<T1>(request, assetObject);
        }
        
        protected abstract Object LoadObjectFromReference<T1>(T assetRef) where T1 : Object;
        
        #endregion

        #region Load Async
        
        protected override IEnumerator LoadAsync<T1>(AssetLoadRequestAsync<T1> request)
        {
            var assetRef = GetReferenceById(request.Id);
            return assetRef == null ? 
                null : 
                LoadObjectFromReferenceAsync(assetRef, request);
        }
        
        protected abstract IEnumerator LoadObjectFromReferenceAsync<T1>(T assetRef, AssetLoadRequestAsync<T1> request)
            where T1 : Object;
        
        protected static void NotifyProgress<T1>(AssetLoadRequestAsync<T1> request, float progress)
        {
            request.OnProgress?.Invoke(progress);
        }

        protected static void NotifyFinish<T1>(AssetLoadRequestAsync<T1> request, Object targetObject) where T1 : Object
        {
            request.OnFinish.Invoke(GetValidResult<T1>(request, targetObject));
        }
        
        #endregion
    }
    
}