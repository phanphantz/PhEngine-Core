using UnityEngine;

namespace PhEngine.Core
{
    public class ServiceGroup : MonoBehaviour
    {
        public virtual bool IsPersistent => true;
       
        void Awake()
        {
            if (IsPersistent && Application.isPlaying)
                DontDestroyOnLoad(gameObject);
        }
    }
}