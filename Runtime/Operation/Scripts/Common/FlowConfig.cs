using UnityEngine;

namespace PhEngine.Core.Operation
{
    public abstract class FlowConfig : ScriptableObject
    {
        public abstract Flow Create();

        [ContextMenu(nameof(RunAsSeries))]
        public void RunAsSeries()
        {
            var flow = Create();
            flow.RunAsSeries();
        }
    }
}
