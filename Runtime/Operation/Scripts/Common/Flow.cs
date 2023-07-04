using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

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

        public Flow(Flow flow)
        {
            operationList.AddRange(flow.Operations);
        }

        public Flow(params Operation[] operations)
        {
            operationList.AddRange(operations);
        }

        public void Add(Operation operation)
        {
            operationList.Add(operation);
        }

        public void Insert(int index, Operation operation)
        {
            operationList.Insert(index, operation);
        }

        public void AddRange(params Operation[] operations)
        {
            operationList.AddRange(operations);
        }

        public void Remove(Operation operation)
        {
            operationList.Remove(operation);
        }

        public Operation Add(Action action)
        {
            var operation = new Operation(action);
            operationList.Add(operation);
            return operation;
        }

        public void Merge(Flow flow)
        {
            AddRange(flow.Operations);
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

        public Flow CreateCopy(Operation startStep = null)
        {
            if (startStep == null)
                return new Flow(this);
            
            var operations = Operations.ToList();
            var startIndex = operations.IndexOf(startStep);
            for (int i = 0; i < startIndex; i++)
                operations.RemoveAt(0);

            return new Flow(operations.ToArray());
        }
    }
}