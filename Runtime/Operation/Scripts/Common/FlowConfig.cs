using UnityEngine;

namespace PhEngine.Core.Operation
{
    public abstract class FlowConfig : ScriptableObject
    {
        public abstract Flow Create();

        [ContextMenu("Test")]
        public void Test()
        {
            RunAsSeries();
        }
        
        public ChainedOperation RunAsSeries()
        {
            var flow = Create();
            return flow.RunAsSeries();
        }
    }
}
