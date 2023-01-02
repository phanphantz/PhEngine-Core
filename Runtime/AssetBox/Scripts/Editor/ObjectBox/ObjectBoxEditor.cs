using PhEngine.Core.AssetBox;
using PhEngine.Core.AssetBox.Editor;
using UnityEditor;
using UnityEngine;

namespace Core.AssetBox.Editor
{
    [CustomEditor(typeof(ObjectBox<>), true)]
    public class ObjectBoxEditor : AssetBoxEditor
    {
        IObjectBox ObjectBox => Component as IObjectBox;
        
        protected override void DrawAssetsSection()
        {
            base.DrawAssetsSection();
            DrawVariantSection();
            EditorGUILayout.Space();
        }
        
        void DrawVariantSection()
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("variants"));
            DrawUpdateButton();
        }
        
        void DrawUpdateButton()
        {
            EditorGUI.BeginDisabledGroup(ObjectBox.Variants == null || ObjectBox.Variants.Length == 0);
            if (GUILayout.Button("Update Variants"))
                ObjectBoxVariantUpdater.Update(ObjectBox);
            EditorGUI.EndDisabledGroup();
        }
    }
}