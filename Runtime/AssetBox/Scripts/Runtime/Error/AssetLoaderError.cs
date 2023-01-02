namespace PhEngine.Core.AssetBox
{
    public static class AssetLoaderError
    {
        public static string GetAssetTypeMismatchErrorMessage<T>(string id)
        {
            return $"Cannot Load {typeof(T).FullName} with id : {id}. the asset is not a type of {typeof(T).FullName}";
        }
        
    }
}