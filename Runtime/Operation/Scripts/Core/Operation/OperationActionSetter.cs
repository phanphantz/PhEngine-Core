using System;

namespace PhEngine.Core.Operation
{
    public static class OperationActionSetter
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

        public static Operation SetOnTimeChange(this Operation operation, Action<TimeSpan> callback)
        {
            operation.SetOnTimeChange(callback);
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
    }
}