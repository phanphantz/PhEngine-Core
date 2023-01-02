using System.Collections.Generic;
using UnityEngine;

namespace PhEngine.Core.Operation
{
    public abstract class OperationRunner<T> : MonoBehaviour where T : Operation
    {
        public IReadOnlyCollection<T> Operations => operationList.AsReadOnly();
        [SerializeField] protected List<T> operationList = new List<T>();
        
        public void Add(T operation)
        {
            operationList.Add(operation);
        }
        
        public void Remove(T operation)
        {
            operationList.Remove(operation);
        }

        public void Clear()
        {
            operationList.Clear();
        }

        [ContextMenu("Run")]
        public abstract void RunAll();

        [ContextMenu("Pause")]
        public abstract void Pause();

        [ContextMenu("Resume")]
        public abstract void Resume();

        [ContextMenu("Force Finish")]
        public abstract void ForceFinish();

        [ContextMenu("Force Cancel")]
        public abstract void ForceCancel();

        protected void RunOperation(Operation operation)
        {
            this.Run(operation);
        }

        protected void PauseOperation(Operation operation)
        {
            operation?.Pause();
        }

        protected void ResumeOperation(Operation operation)
        {
            operation?.Resume();
        }

        protected void ForceFinishOperation(Operation operation)
        {
            operation?.Finish();
        }
        
        protected void ForceCancelOperation(Operation operation)
        {
            operation.Cancel();
        }
    }
}