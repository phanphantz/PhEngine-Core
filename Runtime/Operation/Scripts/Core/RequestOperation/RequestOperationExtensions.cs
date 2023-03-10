using System;

namespace PhEngine.Core.Operation
{
    public static class RequestOperationExtensions
    {
        public static RequestOperation<T> SetOnReceiveResponse<T>(this RequestOperation<T> operation, Action callback)
        {
            operation.SetOnReceiveResponse(callback);
            return operation;
        }
        
        public static RequestOperation<T> SetOnSuccess<T>(this RequestOperation<T> operation, Action<T> callback)
        {
            operation.SetOnSuccess(callback);
            return operation;
        }
        
        public static RequestOperation<T> SetOnFail<T>(this RequestOperation<T> operation, Action<T> callback)
        {
            operation.SetOnFail(callback);
            return operation;
        }
        
        public static RequestOperation<T> SetResultCreation<T>(this RequestOperation<T> operation, Func<T> creator)
        {
            operation.SetResultCreation(creator);
            return operation;
        }
        
        public static RequestOperation<T> SetSuccessCondition<T>(this RequestOperation<T> operation, Func<bool> successCondition)
        {
            operation.SetSuccessCondition(successCondition);
            return operation;
        }
    }
}