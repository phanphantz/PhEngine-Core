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
        [SerializeField] OnStopBehavior onStopBehavior;
        
        public int Count { get; private set; }
        
        public GroupedOperation(OnStopBehavior onStopBehavior = OnStopBehavior.CancelAll, params T[] operations)
        {
            SetOperations(operations);
            SetOnStopBehavior(onStopBehavior);
            ProgressGetter = GetProgress;
            OnStart += BindStepBasedActionsToAllOperations;
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
        
        void BindStepBasedActionsToAllOperations()
        {
            foreach (var operation in operations)
                BindStepBasedActions(operation);
        }

        protected void BindStepBasedActions(T operation)
        {
            //Every bindings to operations need to be removed after canceled and finished
            operation.OnProgress += RefreshStepProgress;
            operation.OnCancel += NotifyStopping;
            operation.OnCancel += UnbindActions;
            operation.OnFinish += UnbindActions;
            
            if (operation is IRequestOperation requestOperation)
                requestOperation.AppendOnFailOneShot(NotifyStopping);
            
            void UnbindActions()
            {
                operation.OnProgress -= RefreshStepProgress;
                operation.OnCancel -= NotifyStopping;
                operation.OnCancel -= UnbindActions;
                operation.OnFinish -= UnbindActions;
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