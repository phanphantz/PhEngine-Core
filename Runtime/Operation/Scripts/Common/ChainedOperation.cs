using System;

namespace PhEngine.Core.Operation
{
    [Serializable]
    public class ChainedOperation : ChainedOperation<Operation>
    {
        public ChainedOperation(OnStopBehavior onStopBehavior = OnStopBehavior.CancelAll, params Operation[] operations) : base (onStopBehavior , operations)
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

        public ChainedOperation(OnStopBehavior onStopBehavior = OnStopBehavior.CancelAll, params T[] operations) : base (onStopBehavior, operations)
        {
            OnStart += RunFirstOperation;
            OnCancel += CancelCurrentOperation;
        }
        
        void RunFirstOperation()
        {
            ChainAllOperationsByOnFinish();
            CurrentStepIndex = 0;
            RunCurrentOperation();
        }
        
        void RunCurrentOperation()
        {
            CurrentOperation?.RunOn(Host);
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

        void ChainAllOperationsByOnFinish()
        {
            for (var i = 0; i < Operations.Length - 1; i++)
                BindRunNextOperation(Operations[i]);
        }

        protected virtual void BindRunNextOperation(T operation)
        {
            if (operation is IRequestOperation requestOperation)
                requestOperation.AppendOnSuccessOneShot(()=>RunNextOperation());
            else
            {
                operation.OnFinish += RunNextOperationOneShot;
                void RunNextOperationOneShot()
                {
                    if (RunNextOperation())
                        operation.OnFinish -= RunNextOperationOneShot;
                }
            }
        }

        protected bool RunNextOperation()
        {
            if (CurrentOperation.IsShouldRepeat())
                return false;
            
            CurrentStepIndex++;
            RunCurrentOperation();
            return true;
        }
    }
}