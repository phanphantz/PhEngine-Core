using System.Linq;
using UnityEngine;

namespace PhEngine.Core.AssetBox
{
    public partial class ResourceBox
    {
#if UNITY_EDITOR
        protected override PathRef CreateRef(AssetRef assetRef)
        {
            var fullPath = assetRef.GetFullPath();
            var loadPath = GetLocalResourcesPath(fullPath);
            return new PathRef(assetRef.id, loadPath, fullPath);
        }

        static string GetLocalResourcesPath(string fullPath)
        {
            var localPathWithExtension = GetLocalResourcePathWithExtension(fullPath);
            return GetPathWithNoFileExtension(localPathWithExtension);
        }

        static string GetLocalResourcePathWithExtension(string fullPath)
        {
            var list = fullPath.Split('/').ToList();
            if (!list.Contains("Resources"))
            {
                Debug.LogError("Failed to get relative resources path from : " + fullPath +
                               "\n The provided path is not in the Resources folder.");
                return string.Empty;
            }

            while (list.Exists(x => x == "Resources"))
                list.RemoveAt(0);

            return string.Join("/", list);
        }

        static string GetPathWithNoFileExtension(string path)
        {
            var index = path.LastIndexOf('.');
            return index < 0 ? path : path.Remove(index);
        }
#endif
    }
}