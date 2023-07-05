using System;
using System.Collections.Generic;
using UnityEngine;

namespace PhEngine.Core.Operation
{
    public class Flow
    {
        public Operation[] Operations => operationList.ToArray();
        readonly List<Operation> operationList = new List<Operation>();

        public event Action OnAnyFail;
        public event Action OnCompleteAll;

        bool isStarted;

        public bool IsBusy;
        
        public Flow()
        {
        }

        public Flow(params Operation[] operations)
        {
            AddRange(operations);
        }

        public void Add(Operation operation)
        {
            operationList.Add(operation);
            operation.SetParentFlow(this);
        }

        public void Insert(int index, Operation operation)
        {
            operationList.Insert(index, operation);
            operation.SetParentFlow(this);
        }

        public void InsertOneShot(int index, Operation operation)
        {
            Insert(index, operation);
            operation.OnFinish += () => Remove(operation);
            OnAnyFail += () => Remove(operation);
            OnCompleteAll += () => Remove(operation);
        }

        public void AddRange(params Operation[] operations)
        {
            foreach (var operation in operations)
                Add(operation);
        }

        public void Remove(Operation operation)
        {
            if (operation.ParentFlow != this)
                return;
            
            operationList.Remove(operation);
            operation.SetParentFlow(null);
        }

        public Operation Add(Action action)
        {
            var operation = new Operation(action);
            Add(operation);
            return operation;
        }

        public void Acquire(Flow flow)
        {
            var oldOperations = flow.ReleaseOperations();
            AddRange(oldOperations);
            AppendActions(flow);
        }

        void AppendActions(Flow flow)
        {
            OnAnyFail += flow.OnAnyFail;
            OnCompleteAll += flow.OnCompleteAll;
        }

        public ChainedOperation RunAsSeries(OnStopBehavior stopBehavior = OnStopBehavior.CancelAll)
        {
            IsBusy = true;
            var chainedOperation = CreateChainOperation(stopBehavior);
            chainedOperation.Run();
            return chainedOperation;
        }
        
        ChainedOperation CreateChainOperation(OnStopBehavior stopBehavior)
        {
            var chainedOperation = new ChainedOperation(stopBehavior, operationList.ToArray());
            chainedOperation.OnCancel += NotifyFail;
            chainedOperation.OnFinish += NotifyComplete;
            return chainedOperation;
        }

        void NotifyComplete()
        {
            OnCompleteAll?.Invoke();
            IsBusy = false;
        }
        
        void NotifyFail()
        {
            OnAnyFail?.Invoke();
            IsBusy = false;
        }

        public ParallelOperation RunAsParallel(OnStopBehavior stopBehavior = OnStopBehavior.Skip)
        {
            IsBusy = true;
            var parallelOperation = CreateParallelOperation(stopBehavior);
            parallelOperation.Run();
            return parallelOperation;
        }
        
        ParallelOperation CreateParallelOperation(OnStopBehavior stopBehavior)
        {
            var parallelOperation = new ParallelOperation(stopBehavior, operationList.ToArray());
            parallelOperation.OnCancel += NotifyFail;
            parallelOperation.OnFinish += NotifyComplete;
            return parallelOperation;
        }
        
        public Operation[] ReleaseOperations()
        {
            IsBusy = false;
            var resultList = new List<Operation>(Operations);
            operationList.Clear();
            foreach (var operation in resultList)
                operation.SetParentFlow(null);
            
            return resultList.ToArray();
        }

        public CustomYieldInstruction WaitSeries()
        {
            RunAsSeries();
            return new WaitWhile(() => IsBusy);
        }

        public CustomYieldInstruction WaitParallel()
        {
            RunAsParallel();
            return new WaitWhile(() => IsBusy);
        }
        
    }
}