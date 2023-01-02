namespace PhEngine.Core.AssetBox
{
    public abstract partial class AssetBox
    {
#if UNITY_EDITOR
        public abstract void Pack(params AssetRef[] assetRefs);
        public abstract string[] GetAssetPaths();
        public abstract AssetRef[] GetBaseAssetReferences();
#endif
    }
}