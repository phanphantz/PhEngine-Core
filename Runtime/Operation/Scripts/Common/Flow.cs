using System;
using System.Collections.Generic;

namespace PhEngine.Core.Operation
{
    public class Flow
    {
        readonly List<Operation> operationList = new List<Operation>();

        public Flow()
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

        public ChainedOperation RunAsSeries(OnStopBehavior stopBehavior = OnStopBehavior.CancelAll)
        {
            return operationList.ToArray().RunAsSeries(stopBehavior);
        }

        public void RunAsParallel()
        {
            operationList.ToArray().RunAsParallel();
        }
    }
}