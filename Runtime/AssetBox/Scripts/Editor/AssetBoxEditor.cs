using PhEngine.Core.AssetBox;
using PhEngine.Core.AssetBox.Editor;
using PhEngine.Core.Editor;
using UnityEditor;
using UnityEngine;

namespace Core.AssetBox.Editor
{
    [CustomEditor(typeof(PhEngine.Core.AssetBox.AssetBox) , true)]
    public abstract class AssetBoxEditor : CustomInspector<PhEngine.Core.AssetBox.AssetBox>
    {
        protected override bool IsDrawDefaultOnInspectorGUI => false;
        protected override void OnPostInspectorGUI()
        {
            DrawAssetsSection();
        }

        protected virtual void DrawAssetsSection()
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("references"));
        }

    }
}