namespace PhEngine.Core.AssetBox
{
    [System.Serializable]
    public abstract partial class AssetRef
    {
        public string id;
        protected AssetRef(string id)
        {
            this.id = id;
        }

    }

}