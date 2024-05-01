using System;
using UnityEngine;

namespace PhEngine.Core.Operation
{
    [Serializable]
    public abstract class GroupedOperation<T> : Operation where T : Operation
    {
        protected T[] Operations => operations;
        [SerializeField] T[] operations;

        public OnStopBehavior OnStopBehavior => onStopBehavior;
        [SerializeField] OnStopBehavior onStopBehavior = OnStopBehavior.CancelAll;
        
        public int Count { get; private set; }

        protected GroupedOperation(OnStopBehavior onStopBehavior, params T[] operations)
        {
            SetOperations(operations);
            SetOnStopBehavior(onStopBehavior);
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

        protected void BindStepBasedActions(T operation)
        {
            //Every bindings to operations need to be removed after canceled and finished
            operation.OnProgress -= RefreshStepProgress;
            operation.OnProgress += RefreshStepProgress;
            
            operation.BindOnCancel(NotifyStopping, true);
            operation.BindOnCancel(UnbindOnProgress, true);
            operation.BindOnFinish(UnbindOnProgress, true);
            
            if (operation is IRequestOperation requestOperation)
                requestOperation.BindOnFailTypeless(NotifyStopping, true);
            
            void UnbindOnProgress()
            {
                operation.OnProgress -= RefreshStepProgress;
            }
        }
        
        protected abstract void RefreshStepProgress(float value);
        protected abstract float GetProgress();
        protected abstract void NotifyStopping();
    }
    
    public enum OnStopBehavior
    {
        CancelAll, Retry, Restart, Skip
    }
}