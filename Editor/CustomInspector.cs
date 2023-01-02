using UnityEngine;
using UnityEditor;
using Object = UnityEngine.Object;

namespace PhEngine.Core.Editor
{
    public abstract class CustomInspector<T> : UnityEditor.Editor
        where T : Object
    {
        protected T Component => component;
        T component;
        
        public object Target => target;
        
        protected abstract bool IsDrawDefaultOnInspectorGUI { get; }
        void OnEnable()
        {
            component = target as T;
            Prepare();
        }

        protected virtual void Prepare() {}

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            DrawContent();
            OnFinishDrawing();
        }

        void DrawContent()
        {
            OnPreInspectorGUI();
            if (IsDrawDefaultOnInspectorGUI)
                base.OnInspectorGUI();

            OnPostInspectorGUI();
        }
        
        protected virtual void OnPreInspectorGUI()
        {
            
        }

        protected virtual void OnPostInspectorGUI()
        {
            
        }
        
        void OnFinishDrawing()
        {
            serializedObject.ApplyModifiedProperties();
            if (GUI.changed)
                MarkAsDirty();
        }
        
        void MarkAsDirty()
        {
            EditorUtility.SetDirty(component);
        }
        
        protected void HandleUndo(string message)
        {
            Undo.RegisterCompleteObjectUndo(component, "Modify " + message);
        }
    }
}