using System;
using System.Linq;

namespace PhEngine.Core.Operation
{
    [Serializable]
    public class ParallelOperation : ParallelOperation<Operation>
    {
        public ParallelOperation(OnStopBehavior stopBehavior, params Operation[] operations) : base(stopBehavior, operations)
        {
        }
        
        public ParallelOperation(params Operation[] operations) : base(operations)
        {
        }
    }
    
    public class ParallelOperation<T> : GroupedOperation<T> where T : Operation
    {
        public float LazyProgress => IsFinished ? 1f : lazyProgress;
        float lazyProgress;
        
        public ParallelOperation(OnStopBehavior stopBehavior, params T[] operations) : base(stopBehavior, operations)
        {
            OnStart += RunAll;
        }
        
        public ParallelOperation(params T[] operations) : base(OnStopBehavior.Skip, operations)
        {
            OnStart += RunAll;
        }

        void RunAll()
        {
            lazyProgress = 0;
            foreach (var operation in Operations)
            {
                BindStepBasedActions(operation);
                operation.Run();
            }
        }

        protected override void RefreshStepProgress(float value)
        {
            lazyProgress = Operations.Sum(op => op.CurrentProgress);
        }

        protected override float GetProgress()
        {
            if (Operations == null || Operations.Length == 0)
                return 0;

            return Operations.Sum(op => op.CurrentProgress) / Operations.Length;
        }

        protected override void NotifyStopping()
        {
            switch (OnStopBehavior)
            {
                case OnStopBehavior.Retry:
                    foreach (var op in Operations)
                        OperationRunnerUtils.Restart(op);
                    break;
                
                case OnStopBehavior.Restart:
                    RunAll();
                    break;
                
                case OnStopBehavior.CancelAll:
                    NotifyCancel();
                    break;
                
                case OnStopBehavior.Skip:
                    break;
                
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}