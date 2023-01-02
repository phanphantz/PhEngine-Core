using System.Reflection;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace PhEngine.Core.Editor
{
    public static class EditorGUIUtils
    {
        public static bool DrawButton(GUIContent content ,bool currentValue, Color color, string tooltip = null, Rect rect = new Rect())
        {
            var tempColor = GUI.color;
            GUI.color = color;
            content.tooltip = tooltip;

            if (rect == Rect.zero)
            {
                if (GUILayout.Button(content, GUILayout.Width(18), GUILayout.Height(18)))
                {
                    return true;
                }
            }
            else
            {
                if (GUI.Button(rect, content))
                {
                    return true;
                }
            }

            GUI.color = tempColor;
            return false;
        }
        
        public static void DrawUILine(int indent = 0, float padding = 3)
        {
            var thickness = 2;
            Rect r = EditorGUILayout.GetControlRect(GUILayout.Height(padding+thickness));
            r.height = thickness;
            r.y+=padding/2;
            r.x-=2;
            r.x += indent;
            r.width +=6;
            EditorGUI.DrawRect(r, Color.grey);
        }
        
        public static void AddHorizontalSpace(float space)
        {
            EditorGUILayout.LabelField( "", GUILayout.Width(space));
        }

        public static bool DrawIndentedButton(float space, string label)
        {
            EditorGUILayout.BeginHorizontal();
            AddHorizontalSpace(space);
            if (GUILayout.Button(label))
                return true;
            
            EditorGUILayout.EndHorizontal();
            return false;
        }

        public static void DrawTitle(string message)
        {
            EditorStyles.label.fontStyle = FontStyle.Bold;
            EditorGUILayout.LabelField(message);
            EditorStyles.label.fontStyle = FontStyle.Normal;
        }

        public static string[] GetSortingLayerNames()
        {
            var internalEditorUtilityType = typeof(InternalEditorUtility);
            var sortingLayersProperty = internalEditorUtilityType.GetProperty("sortingLayerNames", BindingFlags.Static | BindingFlags.NonPublic);
            if (sortingLayersProperty == null)
                return new string[] { };
            
            return (string[])sortingLayersProperty.GetValue(null, new object[0]);
        }
        
        public static void DrawSerializedProperty(SerializedProperty _serializedProperty,  string name, bool isDrawFoldout= true)
        {
            if (_serializedProperty == null)
                return;

            var serializedProperty = _serializedProperty.Copy();
            int startingDepth = serializedProperty.depth;

            if (isDrawFoldout)
                DrawPropertyField(serializedProperty, name);
            else
                serializedProperty.isExpanded = true;
            
            while (serializedProperty.NextVisible(serializedProperty.isExpanded &&
                                                  !PropertyTypeHasDefaultCustomDrawer(serializedProperty
                                                      .propertyType)) && serializedProperty.depth > startingDepth)
            {
                DrawPropertyField(serializedProperty);
            }
        }

        static void DrawPropertyField(SerializedProperty serializedProperty, string name)
        {
            if (serializedProperty.propertyType == SerializedPropertyType.Generic)
            {
                serializedProperty.isExpanded = EditorGUILayout.Foldout(serializedProperty.isExpanded,
                    serializedProperty.displayName, true);
            }
            else
            {
                EditorGUILayout.PropertyField(serializedProperty, new GUIContent(name));
            }
        }
        
        static void DrawPropertyField(SerializedProperty serializedProperty)
        {
            if (serializedProperty.propertyType == SerializedPropertyType.Generic)
            {
                serializedProperty.isExpanded = EditorGUILayout.Foldout(serializedProperty.isExpanded,
                    serializedProperty.displayName, true);
            }
            else
            {
                EditorGUILayout.PropertyField(serializedProperty);
            }
        }

        static bool PropertyTypeHasDefaultCustomDrawer(SerializedPropertyType type)
        {
            return
                type == SerializedPropertyType.AnimationCurve ||
                type == SerializedPropertyType.Bounds ||
                type == SerializedPropertyType.Color ||
                type == SerializedPropertyType.Gradient ||
                type == SerializedPropertyType.LayerMask ||
                type == SerializedPropertyType.ObjectReference ||
                type == SerializedPropertyType.Rect ||
                type == SerializedPropertyType.Vector2 ||
                type == SerializedPropertyType.Vector3;
        }
    }
}