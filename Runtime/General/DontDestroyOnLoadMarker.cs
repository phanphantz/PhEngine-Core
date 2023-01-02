using UnityEngine;

namespace PhEngine.Core
{
    public class DontDestroyOnLoadMarker : MonoBehaviour
    {
        void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }
    }
}