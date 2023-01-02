namespace PhEngine.Core.AssetBox
{
    public static class AssetBoxError
    {
        public static string GetRefNotFoundErrorMessage(string loadId, string name)
        {
            return $"Cannot Load asset. There is no reference with Id : '{loadId}' in '{name}'";
        }
        
        public static string GetAssetNotFoundErrorMessage(string loadId, string name )
        {
            return $"Cannot Load asset with id : '{loadId}' from '{name}'. The asset is not found at the source.";
        }
        
    }
}