using System;

namespace PhEngine.Core.Operation
{
    public static class ActionUtils
    {
        public static T BindOnStart<T>(this T operation, Action callback, bool isOneShot = false) where T : Operation
        {
            operation.BindOnStart(callback, isOneShot);
            return operation;
        }

        public static T BindOnUpdate<T>(this T operation, Action callback, bool isOneShot = false) where T : Operation
        {
            operation.BindOnUpdate(callback, isOneShot);
            return operation;
        }

        public static T BindOnProgress<T>(this T operation, Action<float> callback, bool isOneShot = false) where T : Operation
        {
            operation.BindOnProgress(callback, isOneShot);
            return operation;
        }

        public static T BindOnFinish<T>(this T operation, Action callback, bool isOneShot = false) where T : Operation
        {
            operation.BindOnFinish(callback, isOneShot);
            return operation;
        }

        public static T BindOnCancel<T>(this T operation, Action callback, bool isOneShot = false) where T : Operation
        {
            operation.BindOnCancel(callback, isOneShot);
            return operation;
        }
        
        public static T BindOnPause<T>(this T operation, Action callback, bool isOneShot = false) where T : Operation
        {
            operation.BindOnPause(callback, isOneShot);
            return operation;
        }
        
        public static T BindOnResume<T>(this T operation, Action callback, bool isOneShot = false) where T : Operation
        {
            operation.BindOnResume(callback, isOneShot);
            return operation;
        }
    }
}