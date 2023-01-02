using UnityEngine;

namespace PhEngine.Core.Operation
{
    public static class OperationRunnerUtils
    {
        public static void RunIfNotActive(this MonoBehaviour target, Operation operation)
        {
            operation.RunOnIfNotActive(target);
        }
        
        public static void Run(this MonoBehaviour target, Operation operation)
        {
            operation.RunOn(target);
        }
        
        public static void Restart(this MonoBehaviour target, Operation operation)
        {
            operation.Restart(target);
        }
        
        public static ChainedOperation RunAsSeries(this MonoBehaviour target, OnStopBehavior onStopBehavior = OnStopBehavior.CancelAll, params Operation[] operations)
        {
            var chainedOperation = new ChainedOperation(onStopBehavior,operations);
            Run(target,chainedOperation);
            return chainedOperation;
        }
        
        public static ChainedOperation<T> RunAsSeries<T>(this MonoBehaviour target, OnStopBehavior onStopBehavior = OnStopBehavior.CancelAll, params T[] operations) where T : Operation
        {
            var chainedOperation = new ChainedOperation<T>(onStopBehavior,operations);
            Run(target,chainedOperation);
            return chainedOperation;
        }

        public static void RunAsParallel(this MonoBehaviour target, params Operation[] operations)
        {
            RunAsParallel<Operation>(target, operations);
        }
        
        public static void RunAsParallel<T>(this MonoBehaviour target, params T[] operations) where T : Operation
        {
            foreach (var operation in operations)
                Run(target,operation);
        }
    }
}