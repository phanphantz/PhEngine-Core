using UnityEngine;

namespace PhEngine.Core.Operation
{
    public static class OperationRunnerUtils
    {
        static MonoBehaviour MasterTarget
        {
            get
            {
                if (!Application.isPlaying)
                    return null;

                return MasterOperationRunner.Instance;
            }
        }
        
        public static void RunIfNotActive(this Operation operation)
        {
            MasterTarget.RunIfNotActive(operation);
        }
        
        public static void Run(this Operation operation)
        {
            operation.RunOn(MasterTarget);
        }
        
        public static void Restart(this Operation operation)
        {
            operation.RestartOn(MasterTarget);
        }
        
        public static ChainedOperation RunAsSeries(this Operation[] operations, OnStopBehavior onStopBehavior = OnStopBehavior.CancelAll)
        {
            return MasterTarget.RunAsSeries(onStopBehavior, operations);
        }
        
        public static ChainedOperation<T> RunAsSeries<T>(this T[] operations, OnStopBehavior onStopBehavior = OnStopBehavior.CancelAll) where T : Operation
        {
            return MasterTarget.RunAsSeries<T>(onStopBehavior, operations);
        }

        public static void RunAsParallel(this Operation[] operations)
        {
            MasterTarget.RunAsParallel(operations);
        }
        
        public static void RunAsParallel<T>(this T[] operations) where T : Operation
        {
            MasterTarget.RunAsParallel<Operation>(operations);
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