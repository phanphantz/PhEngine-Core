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

        public static void Run(this Operation operation)
        {
            operation.RunOn(MasterTarget);
        }
        
        public static void Restart(this Operation operation)
        {
            Restart(operation);
        }
        
        public static ChainedOperation RunAsSeries(this Operation[] operations, OnStopBehavior onStopBehavior = OnStopBehavior.CancelAll)
        {
            return MasterTarget.RunAsSeries(onStopBehavior, operations);
        }
        
        public static ChainedOperation<T> RunAsSeries<T>(this T[] operations, OnStopBehavior onStopBehavior = OnStopBehavior.CancelAll) where T : Operation
        {
            return MasterTarget.RunAsSeries<T>(onStopBehavior, operations);
        }

        public static ParallelOperation RunAsParallel(this Operation[] operations, OnStopBehavior onStopBehavior = OnStopBehavior.Skip)
        {
            return MasterTarget.RunAsParallel(onStopBehavior, operations);
        }
        
        public static ParallelOperation<T> RunAsParallel<T>(this T[] operations, OnStopBehavior onStopBehavior = OnStopBehavior.Skip) where T : Operation
        {
            return MasterTarget.RunAsParallel(onStopBehavior, operations);
        }
        
        public static void Run(this MonoBehaviour target, Operation operation)
        {
            operation.RunOn(target);
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

        public static ParallelOperation RunAsParallel(this MonoBehaviour target, OnStopBehavior onStopBehavior = OnStopBehavior.Skip, params Operation[] operations)
        {
            var parallelOperation = new ParallelOperation(onStopBehavior, operations);
            Run(target, parallelOperation);
            return parallelOperation;
        }
        
        public static ParallelOperation<T> RunAsParallel<T>(this MonoBehaviour target, OnStopBehavior onStopBehavior = OnStopBehavior.Skip, params T[] operations) where T : Operation
        {
            var parallelOperation = new ParallelOperation<T>(onStopBehavior, operations);
            Run(target, parallelOperation);
            return parallelOperation;
        }
    }
}