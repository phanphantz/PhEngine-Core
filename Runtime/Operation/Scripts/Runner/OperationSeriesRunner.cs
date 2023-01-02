using System.Linq;
using UnityEngine;

namespace PhEngine.Core.Operation
{
    public abstract class OperationSeriesRunner<T> : OperationRunner<T> where T : Operation
    {
        ChainedOperation<T> chainedOperation;
        [SerializeField] OnStopBehavior onStopBehavior;
        
        public override void RunAll()
        {
            if (chainedOperation != null)
                chainedOperation.Cancel();

            chainedOperation = this.RunAsSeries(onStopBehavior, Operations.ToArray());
        }

        public override void Pause()
        {
            if (!ValidateOperation())
                return;
            
            PauseOperation(chainedOperation);
        }

        bool ValidateOperation()
        {
            if (chainedOperation != null) 
                return true;
            
            Debug.Log("Action is invalid. There is no ChainedOperation running.");
            return false;
        }

        public override void Resume()
        {
            if (!ValidateOperation())
                return;
            
            ResumeOperation(chainedOperation);
        }

        public override void ForceFinish()
        {
            if (!ValidateOperation())
                return;
            
            ForceFinishOperation(chainedOperation);
        }

        public override void ForceCancel()
        {
            if (!ValidateOperation())
                return;
            
            ForceCancelOperation(chainedOperation);
        }
    }
}