using System;

namespace PhEngine.Core.Operation
{
    [Serializable]
    public class ChainedOperation : ChainedOperation<Operation>
    {
        public ChainedOperation(OnStopBehavior onStopBehavior, params Operation[] operations) : base (onStopBehavior , operations)
        {
        }
        
        public ChainedOperation(params Operation[] operations) : base (operations)
        {
        }
    }

    [Serializable]
    public class ChainedOperation<T> : GroupedOperation<T> where T : Operation
    {
        public float CurrentStepProgress { get; private set; }
        public int CurrentStepIndex { get; private set; }
        
        T CurrentOperation
        {
            get
            {
                if (CurrentStepIndex >= Operations.Length)
                    return default;
                
                return Operations[CurrentStepIndex];
            }
        }

        public ChainedOperation(OnStopBehavior onStopBehavior, params T[] operations) : base (onStopBehavior, operations)
        {
            OnStart += RunFirstOperation;
            OnCancel += CancelCurrentOperation;
        }
        
        public ChainedOperation(params T[] operations) : base (OnStopBehavior.CancelAll,operations)
        {
            OnStart += RunFirstOperation;
            OnCancel += CancelCurrentOperation;
        }
        
        void RunFirstOperation()
        {
            CurrentStepIndex = 0;
            RunCurrentOperation();
        }
        
        void RunCurrentOperation()
        {
            if (CurrentOperation == null)
                return;
            
            BindRunNextOperation(CurrentOperation);
            CurrentOperation.RunOn(Host);
        }

        void CancelCurrentOperation()
        {
            CurrentOperation?.Cancel();
        }

        protected override void RefreshStepProgress(float progress)
        {
            CurrentStepProgress = progress;
        }

        protected override void NotifyStopping()
        {
            switch (OnStopBehavior)
            {
                case OnStopBehavior.Retry:
                    RunCurrentOperation();
                    break;
                
                case OnStopBehavior.Restart:
                    RunFirstOperation();
                    break;
                
                case OnStopBehavior.CancelAll:
                    NotifyCancel();
                    break;
                
                case OnStopBehavior.Skip:
                    RunNextOperation();
                    break;
                
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        
        protected override float GetProgress()
        {
            if (Count == 0)
                return 0;
            
            return (CurrentStepIndex + CurrentStepProgress) / Count;
        }

        protected virtual void BindRunNextOperation(T operation)
        {
            BindStepBasedActions(operation);
            if (operation is IRequestOperation requestOperation)
                requestOperation.BindOnSuccessTypeless(RunNextOperation, true);
            else
                operation.BindOnFinish(RunNextOperation, true);
        }

        protected void RunNextOperation()
        {
            CurrentStepIndex++;
            RunCurrentOperation();
        }
    }
}