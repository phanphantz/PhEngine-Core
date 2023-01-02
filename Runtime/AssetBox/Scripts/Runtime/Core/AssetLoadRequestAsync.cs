using System;
using UnityEngine;

namespace PhEngine.Core.AssetBox
{
    public class AssetLoadRequestAsync<T> : AssetLoadRequest
    {
        public Action<T> OnFinish { get; }
        public Action<float> OnProgress { get; }
        
        public AssetLoadRequestAsync(string id, AssetLoadMode mode, Transform parentTransform, Action<T> onFinish, Action<float> onProgress) : base(id, mode, parentTransform)
        {
            OnFinish = onFinish;
            OnProgress = onProgress;
        }
    }
}