namespace PhEngine.Core.AssetBox
{
    public partial class PathRef
    {
#if UNITY_EDITOR
        public override string GetFullPath()
        {
            return fullPath;
        }
#endif
    }
}