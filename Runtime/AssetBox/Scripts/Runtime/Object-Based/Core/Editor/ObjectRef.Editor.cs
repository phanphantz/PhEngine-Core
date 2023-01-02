using UnityEditor;

namespace PhEngine.Core.AssetBox
{
    public partial class ObjectRef<T>
    {
#if UNITY_EDITOR
        public override string GetFullPath()
        {
            return AssetDatabase.GetAssetPath(targetObject);
        }
#endif
    }
    
}