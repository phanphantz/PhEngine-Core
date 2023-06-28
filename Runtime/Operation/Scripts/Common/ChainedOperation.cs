using System;
using UnityEngine;

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
    public class ChainedOperation<T> : Operation where T : Operation
    {
        [SerializeField] T[] operations;
        [SerializeField] OnStopBehavior onStopBehavior;
        public int Count { get; private set; }
        public float CurrentStepProgress { get; private set; }
        public int CurrentStepIndex { get; private set; }

        T CurrentOperation
        {
            get
            {
                if (CurrentStepIndex >= operations.Length)
                    return default;
                
                return operations[CurrentStepIndex];
            }
        }

        public ChainedOperation(OnStopBehavior onStopBehavior = OnStopBehavior.CancelAll, params T[] operations)
        {
            SetOperations(operations);
            SetOnStopBehavior(onStopBehavior);
            OnStart += RunFirstOperation;
            OnCancel += CancelCurrentOperation;
            BindStepBasedActionsToAllOperations();
            ChainAllOperationsByOnFinish();
            ProgressGetter = GetProgress;
        }

        public void SetOnStopBehavior(OnStopBehavior value)
        {
            onStopBehavior = value;
        }

        public void SetOperations(params T[] values)
        {
            operations = values;
            Count = operations.Length;
        }

        void RunFirstOperation()
        {
            CurrentStepIndex = 0;
            RunCurrentOperation();
        }
        
        void RunCurrentOperation()
        {
            var currentOperation = CurrentOperation;
            if (currentOperation == null)
                return;
            
            currentOperation.RunOn(Host);
        }

        void CancelCurrentOperation()
        {
            var currentOperation = CurrentOperation;
            if (currentOperation == null)
                return;
            
            currentOperation.OnCancel -= NotifyStopping;
            currentOperation.Cancel();
        }

        void BindStepBasedActionsToAllOperations()
        {
            foreach (var operation in operations)
                BindStepBasedActions(operation);
        }

        protected virtual void BindStepBasedActions(T operation)
        {
            operation.OnProgress += RefreshStepProgress;
            operation.OnCancel += NotifyStopping;
            if (operation is IRequestOperation requestOperation)
                requestOperation.AppendOnFail(NotifyStopping);
        }

        void RefreshStepProgress(float progress)
        {
            CurrentStepProgress = progress;
        }

        protected void NotifyStopping()
        {
            switch (onStopBehavior)
            {
                case OnStopBehavior.Retry:
                    RunCurrentOperation();
                    break;
                
                case OnStopBehavior.Restart:
                    RunFirstOperation();
                    break;
                
                case OnStopBehavior.CancelAll:
                    ForceCancel();
                    break;
                
                case OnStopBehavior.Skip:
                    RunNextOperation();
                    break;
                
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        void ChainAllOperationsByOnFinish()
        {
            for (var i = 0; i < operations.Length - 1; i++)
                BindRunNextOperation(operations[i]);
        }

        protected virtual void BindRunNextOperation(T operation)
        {
            if (operation is IRequestOperation requestOperation)
                requestOperation.AppendOnSuccess(RunNextOperation);
            else
                operation.OnFinish += RunNextOperation;
        }

        protected void RunNextOperation()
        {
            CurrentStepIndex++;
            RunCurrentOperation();
        }

        float GetProgress()
        {
            if (Count == 0)
                return 0;
            
            return (CurrentStepIndex + CurrentStepProgress) / Count;
        }
    }
    
    public enum OnStopBehavior
    {
        CancelAll, Retry, Restart, Skip
    }
}