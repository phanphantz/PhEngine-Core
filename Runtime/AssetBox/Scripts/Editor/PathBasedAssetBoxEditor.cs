using System.IO;
using PhEngine.Core.AssetBox;
using PhEngine.Core.Editor;
using UnityEditor;

namespace Core.AssetBox.Editor
{
    [CustomEditor(typeof(PathBasedAssetBox), true)]
    public class PathBasedAssetBoxEditor : AssetBoxEditor
    {
        protected IPathBasedAssetBox PathBasedAssetBox => Component as IPathBasedAssetBox;
        bool IsUseEditorVersion() => PathBasedAssetBox.IsUseEditorVersion;

        protected override void OnPostInspectorGUI()
        {
            DrawEditorVersionSection();
            EditorGUILayout.Space();
            base.OnPostInspectorGUI();
        }
        
        void DrawEditorVersionSection()
        {
            EditorGUIUtils.DrawTitle("Editor Settings");
            EditorGUI.indentLevel++;
            DrawEditorFallbackSettings();
            EditorGUI.BeginDisabledGroup(!IsUseEditorVersion());
            EditorGUILayout.PropertyField(serializedObject.FindProperty("editorVersion"));
            EditorGUI.EndDisabledGroup();
            EditorGUI.indentLevel--;
        }

        protected virtual void DrawEditorFallbackSettings()
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("isUseEditorVersion"));
        }

        protected override void DrawAssetsSection()
        {
            EditorGUI.BeginDisabledGroup(IsUseEditorVersion());
            base.DrawAssetsSection();
            EditorGUI.EndDisabledGroup();
        }
        
    }
    
}