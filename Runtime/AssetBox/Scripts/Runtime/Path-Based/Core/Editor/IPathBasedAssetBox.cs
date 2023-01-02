namespace PhEngine.Core.AssetBox
{
    public interface IPathBasedAssetBox
    {
#if UNITY_EDITOR
        bool IsUseEditorVersion { get; }
        void SetIsUseEditorVersion(bool value);
#endif
    }
}