using UnityEngine;

namespace PhEngine.Core.AssetBox
{
    public class AssetLoadRequest
    {
        public string Id { get; }
        public AssetLoadMode Mode { get; }
        public Transform ParentTransform { get; }
        
        public AssetLoadRequest(string id, AssetLoadMode mode, Transform parentTransform)
        {
            Id = id;
            Mode = mode;
            ParentTransform = parentTransform;
        }
    }
}