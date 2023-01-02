using UnityEngine;

namespace PhEngine.Core.AssetBox.Editor
{
    public abstract class AssetNameRule : ScriptableObject
    {
        public abstract string GetName(string path, Object targetObject);
    }
}