using PhEngine.Core.Editor;
using UnityEditor;
using UnityEngine;

namespace PhEngine.Core.AssetBox.Editor
{
    [CustomEditor(typeof(AssetBoxPacker), true), CanEditMultipleObjects]
    public class AssetBoxPackerEditor : CustomInspector<AssetBoxPacker>
    {
        protected override bool IsDrawDefaultOnInspectorGUI => true;
        protected override void OnPostInspectorGUI()
        {
            if (GUILayout.Button("Pack"))
                Component.PackBySelectedType();
        }
        
    }
}