using UnityEngine;

namespace PhEngine.Core.Operation
{
    public static class OperationRunnerUtils
    {
        public static void RunIfNotActive(this Operation operation)
        {
            MasterOperationRunner.Instance.RunIfNotActive(operation);
        }
        
        public static void Run(this Operation operation)
        {
            operation.RunOn(MasterOperationRunner.Instance);
        }
        
        public static void Restart(this Operation operation)
        {
            operation.RestartOn(MasterOperationRunner.Instance);
        }
        
        public static ChainedOperation RunAsSeries(this Operation[] operations, OnStopBehavior onStopBehavior = OnStopBehavior.CancelAll)
        {
            return MasterOperationRunner.Instance.RunAsSeries(onStopBehavior, operations);
        }
        
        public static ChainedOperation<T> RunAsSeries<T>(this T[] operations, OnStopBehavior onStopBehavior = OnStopBehavior.CancelAll) where T : Operation
        {
            return MasterOperationRunner.Instance.RunAsSeries<T>(onStopBehavior, operations);
        }

        public static void RunAsParallel(this Operation[] operations)
        {
            MasterOperationRunner.Instance.RunAsParallel(operations);
        }
        
        public static void RunAsParallel<T>(this T[] operations) where T : Operation
        {
            MasterOperationRunner.Instance.RunAsParallel<Operation>(operations);
        }
        
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
            operation.RestartOn(target);
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