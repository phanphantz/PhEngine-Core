using UnityEngine;

namespace PhEngine.Core.Operation
{
    public class MasterOperationRunner : MonoBehaviour
    {
        public static MasterOperationRunner Instance
        {
            get
            {
                if (unsafeInstance == null)
                {
                    var newObj = new GameObject { name ="[MasterOperationRunner]"};
                    unsafeInstance = newObj.AddComponent<MasterOperationRunner>();
                }

                return unsafeInstance;
            }   
        }

        static MasterOperationRunner unsafeInstance;

        void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }
    }
}