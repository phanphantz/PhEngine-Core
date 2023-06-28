using System;
using System.Collections.Generic;

namespace PhEngine.Core.Operation
{
    public class Sequence
    {
        readonly List<Operation> operationList = new List<Operation>();

        public Sequence()
        {
        }

        public void Add(Operation operation)
        {
            operationList.Add(operation);
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

        public ChainedOperation Run(OnStopBehavior stopBehavior = OnStopBehavior.CancelAll)
        {
            var operation = CreateChainOperation(stopBehavior);
            operation.Run();
            return operation;
        }

        public ChainedOperation CreateChainOperation(OnStopBehavior stopBehavior = OnStopBehavior.CancelAll)
        {
            return new ChainedOperation(stopBehavior,operationList.ToArray());
        }
    }
}