using UnityEngine;

namespace PhEngine.Core
{
    public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        static T instance;
        public static T Instance
        {
            get
            {
                if (instance == null)
                    instance = (T)FindObjectOfType(typeof(T));

                return instance;
            }
            protected set => instance = value;
        }
        
        protected virtual void Awake()
        {
            if (instance != null && instance != this)
            {
                DestroyImmediate(gameObject);
                return;
            }

            instance = GetComponent<T>();
            DontDestroyOnLoad(gameObject.transform.root.gameObject);
            InitAfterAwake();
        }

        protected abstract void InitAfterAwake();
    }
}