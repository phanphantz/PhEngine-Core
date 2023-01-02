namespace PhEngine.Core.AssetBox
{
    public abstract partial class AssetRef
    {
#if UNITY_EDITOR
        public abstract string GetFullPath();
#endif
    }
}