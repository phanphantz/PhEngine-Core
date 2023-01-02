using System.Linq;

namespace PhEngine.Core.AssetBox
{
    public abstract partial class AssetBoxGen<T>
    {
#if UNITY_EDITOR
        public override void Pack(params AssetRef[] assetRefs)
            => Pack(assetRefs.Select(CreateRef).ToArray());

        protected abstract T CreateRef(AssetRef assetRef);

        public override string[] GetAssetPaths()
            => References.Select(reference => reference.GetFullPath()).ToArray();

        public override AssetRef[] GetBaseAssetReferences()
            => References.Select(reference => reference as AssetRef).ToArray();
#endif
    }
}