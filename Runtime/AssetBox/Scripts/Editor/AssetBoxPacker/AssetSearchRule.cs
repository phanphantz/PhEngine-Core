using UnityEngine;

namespace PhEngine.Core.AssetBox.Editor
{
    public abstract class AssetSearchRule : ScriptableObject
    {
        public abstract bool IsPass(string path, Object targetObject);
    }
}