using System;
using System.Collections;
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
        
        public Flow()
        {
        }

        public Flow(params Operation[] operations)
        {
            AddRange(operations);
        }

        public void Add(Operation operation)
        {
            if (operation.ParentFlow != null)
            {
                Debug.LogError("Cannot add operation because it already in another flow.");
                return;
            }
            
            operationList.Add(operation);
            operation.SetParentFlow(this);
        }

        public void Insert(int index, Operation operation)
        {
            if (operation.ParentFlow != null)
            {
                Debug.LogError("Cannot add operation because it already in another flow.");
                return;
            }
            
            operationList.Insert(index, operation);
            operation.SetParentFlow(this);
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

        public void Merge(Flow flow)
        {
            var oldOperations = flow.Disintegrate();
            AddRange(oldOperations.ToArray());
            AppendActions(flow);
        }

        void AppendActions(Flow flow)
        {
            OnAnyFail += flow.OnAnyFail;
            OnCompleteAll += flow.OnCompleteAll;
        }

        public ChainedOperation RunAsSeries(OnStopBehavior stopBehavior = OnStopBehavior.CancelAll)
        {
            var chainedOperation = CreateChainOperation(stopBehavior);
            chainedOperation.Run();
            return chainedOperation;
        }

        public IEnumerator AsChainedCoroutine(OnStopBehavior stopBehavior = OnStopBehavior.CancelAll) =>
            CreateChainOperation(stopBehavior).Coroutine();

        public ChainedOperation CreateChainOperation(OnStopBehavior stopBehavior)
        {
            var chainedOperation = new ChainedOperation(stopBehavior, operationList.ToArray());
            chainedOperation.OnCancel += OnAnyFail;
            chainedOperation.OnFinish += OnCompleteAll;
            return chainedOperation;
        }
        
        public IEnumerator AsParallelCoroutine(OnStopBehavior stopBehavior = OnStopBehavior.Skip) =>
            CreateParallelOperation(stopBehavior).Coroutine();

        public ParallelOperation RunAsParallel(OnStopBehavior stopBehavior = OnStopBehavior.Skip)
        {
            var parallelOperation = CreateParallelOperation(stopBehavior);
            parallelOperation.Run();
            return parallelOperation;
        }
        
        public ParallelOperation CreateParallelOperation(OnStopBehavior stopBehavior)
        {
            var parallelOperation = new ParallelOperation(stopBehavior, operationList.ToArray());
            parallelOperation.OnCancel += OnAnyFail;
            parallelOperation.OnFinish += OnCompleteAll;
            return parallelOperation;
        }

        public void Recycle(Operation startStep)
        {
            var startIndex = operationList.IndexOf(startStep);
            for (int i = 0; i < startIndex; i++)
                operationList.RemoveAt(0);
        }

        public List<Operation> Disintegrate()
        {
            var resultList = new List<Operation>(Operations);
            operationList.Clear();
            foreach (var operation in resultList)
                operation.SetParentFlow(null);
            
            return resultList;
        }
    }
}