using UnityEngine;

namespace PhEngine.Core.AssetBox
{
    public abstract partial class PathBasedAssetBox
    {
#if UNITY_EDITOR
        public bool IsUseEditorVersion => isUseEditorVersion;
        [SerializeField] bool isUseEditorVersion;
       
        public void SetIsUseEditorVersion(bool value) => isUseEditorVersion = value;
        [SerializeField] AssetBox editorVersion;

        protected override AssetBox GetLoadTarget()
        {
            return isUseEditorVersion ? editorVersion : this;
        }
#endif
    }
}