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
        public Operation RunningOperation { get; private set; }
        public bool IsBusy => RunningOperation != null;
        
        public Flow()
        {
        }

        public Flow(params Operation[] operations)
        {
            AddRange(operations);
        }

        public Flow Add(Operation operation)
        {
            operationList.Add(operation);
            operation.SetParentFlow(this);
            return this;
        }

        public Flow Insert(int index, Operation operation)
        {
            operationList.Insert(index, operation);
            operation.SetParentFlow(this);
            return this;
        }

        public Flow InsertBefore(Operation existingStep, Operation operationToInsert)
        {
            return InsertOneShot(operationList.IndexOf(existingStep), operationToInsert);
        }

        public Flow InsertOneShot(int index, Operation operation)
        {
            Insert(index, operation);
            operation.OnFinish += () => Remove(operation);
            operation.OnCancel += () => Remove(operation);
            return this;
        }

        public Flow AddRange(params Operation[] operations)
        {
            foreach (var operation in operations)
                Add(operation);
            return this;
        }

        public void Remove(Operation operation)
        {
            if (operation.ParentFlow != this)
                return;
            
            operationList.Remove(operation);
            operation.SetParentFlow(null);
        }

        public Flow Add(Action action)
        {
            var operation = new Operation(action);
            Add(operation);
            return this;
        }

        public Flow AbsorbFrom(Flow flow)
        {
            var oldOperations = flow.ReleaseOperations();
            AddRange(oldOperations);
            AppendActions(flow);
            return this;
        }

        void AppendActions(Flow flow)
        {
            OnAnyFail += flow.OnAnyFail;
            OnCompleteAll += flow.OnCompleteAll;
        }

        public ChainedOperation RunAsSeries(OnStopBehavior stopBehavior = OnStopBehavior.CancelAll, int startIndex = 0)
        {
            var chainedOperation = CreateChainOperation(stopBehavior, startIndex);
            chainedOperation.Run();
            RunningOperation = chainedOperation;
            return chainedOperation;
        }
        
        ChainedOperation CreateChainOperation(OnStopBehavior stopBehavior, int startIndex = 0)
        {
            var targetOperationList = new List<Operation>(operationList);
            for (int i = 0; i < startIndex; i++)
                targetOperationList.RemoveAt(0);

            var chainedOperation = new ChainedOperation(stopBehavior, targetOperationList.ToArray());
            chainedOperation.OnCancel += NotifyFail;
            chainedOperation.OnFinish += NotifyComplete;
            return chainedOperation;
        }

        void NotifyComplete()
        {
            OnCompleteAll?.Invoke();
            RunningOperation = null;
        }
        
        void NotifyFail()
        {
            OnAnyFail?.Invoke();
            RunningOperation = null;
        }

        public ParallelOperation RunAsParallel(OnStopBehavior stopBehavior = OnStopBehavior.Skip)
        {
            var parallelOperation = CreateParallelOperation(stopBehavior);
            parallelOperation.Run();
            RunningOperation = parallelOperation;
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
            RunningOperation = null;
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

        public static Flow Create()
        {
            return new Flow();
        }
    }
}