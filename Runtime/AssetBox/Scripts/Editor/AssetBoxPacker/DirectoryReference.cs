using System;

namespace PhEngine.Core.AssetBox.Editor
{
    [Serializable]
    public class DirectoryReference
    {
        public string path;
        public bool isIncludeSubFolders;
    }
}