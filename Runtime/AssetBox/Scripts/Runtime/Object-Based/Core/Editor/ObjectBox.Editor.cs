using UnityEditor;
using UnityEngine;

namespace PhEngine.Core.AssetBox
{
    public abstract partial class ObjectBox<T>
    {
#if UNITY_EDITOR
        public AssetBox[] Variants => variants;
        [SerializeField] AssetBox[] variants;

        protected override ObjectRef<T> CreateRef(AssetRef assetRef)
        {
            var fullPath = assetRef.GetFullPath();
            var targetObject = AssetDatabase.LoadAssetAtPath<T>(fullPath);
            return new ObjectRef<T>
            (
                assetRef.id,
                targetObject
            );
        }
#endif
    }
}