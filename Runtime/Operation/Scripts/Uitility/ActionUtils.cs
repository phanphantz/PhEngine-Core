using System;

namespace PhEngine.Core.Operation
{
    public static class ActionUtils
    {
        public static Operation SetOnStart(this Operation operation, Action callback)
        {
            operation.SetOnStart(callback);
            return operation;
        }

        public static Operation SetOnUpdate(this Operation operation, Action callback)
        {
            operation.SetOnUpdate(callback);
            return operation;
        }

        public static Operation SetOnElapsedDeltaTimeChange(this Operation operation, Action<TimeSpan> callback)
        {
            operation.SetOnElapsedDeltaTimeChange(callback);
            return operation;
        }

        public static Operation SetOnProgress(this Operation operation, Action<float> callback)
        {
            operation.SetOnProgress(callback);
            return operation;
        }

        public static Operation SetOnFinish(this Operation operation, Action callback)
        {
            operation.SetOnFinish(callback);
            return operation;
        }

        public static Operation SetOnCancel(this Operation operation, Action callback)
        {
            operation.SetOnCancel(callback);
            return operation;
        }
        
        public static Operation SetOnPause(this Operation operation, Action callback)
        {
            operation.SetOnPause(callback);
            return operation;
        }
        
        public static Operation SetOnResume(this Operation operation, Action callback)
        {
            operation.SetOnResume(callback);
            return operation;
        }
        
        public static Operation BindOneShotOnStart(this Operation operation, Action callback)
        {
            operation.BindOneShotOnStart(callback);
            return operation;
        }

        public static Operation BindOneShotOnUpdate(this Operation operation, Action callback)
        {
            operation.BindOneShotOnUpdate(callback);
            return operation;
        }

        public static Operation BindOneShotOnElapsedDeltaTime(this Operation operation, Action<TimeSpan> callback)
        {
            operation.BindOneShotOnElapsedDeltaTime(callback);
            return operation;
        }

        public static Operation BindOneShotOnProgress(this Operation operation, Action<float> callback)
        {
            operation.BindOneShotOnProgress(callback);
            return operation;
        }

        public static Operation BindOneShotOnFinish(this Operation operation, Action callback)
        {
            operation.BindOneShotOnFinish(callback);
            return operation;
        }

        public static Operation BindOneShotOnCancel(this Operation operation, Action callback)
        {
            operation.BindOneShotOnCancel(callback);
            return operation;
        }
        
        public static Operation BindOneShotOnPause(this Operation operation, Action callback)
        {
            operation.BindOneShotOnPause(callback);
            return operation;
        }
        
        public static Operation BindOneShotOnResume(this Operation operation, Action callback)
        {
            operation.BindOneShotOnResume(callback);
            return operation;
        }
    }
}