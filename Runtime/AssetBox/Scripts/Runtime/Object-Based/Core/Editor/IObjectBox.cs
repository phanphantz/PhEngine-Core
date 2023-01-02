namespace PhEngine.Core.AssetBox
{
    public interface IObjectBox
    {
#if UNITY_EDITOR
        AssetBox[] Variants { get; }
#endif
    }
}