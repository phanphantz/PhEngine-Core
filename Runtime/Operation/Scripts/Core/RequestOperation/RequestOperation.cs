using System;
using UnityEngine.Events;

namespace PhEngine.Core.Operation
{
    [Serializable]
    public class RequestOperation<T> : Operation, IRequestOperation
    {
        void IRequestOperation.AppendOnFail(Action callback)
        {
            OnFail += (val) => callback.Invoke();
        }

        void IRequestOperation.AppendOnSuccess(Action callback)
        {
            OnSuccess += (val) => callback.Invoke();
        }
        
        public event Action<T> OnSuccess;
        public event Action<T> OnFail;
        public event Action OnReceiveResponse;
        
        public UnityEvent<T> onSuccessEvent = new UnityEvent<T>();
        public UnityEvent<T> onFailEvent = new UnityEvent<T>();
        public UnityEvent onReceiveResponseEvent = new UnityEvent();

        public Func<bool> SuccessCondition { get; protected set; }
        public Func<T> ResultCreation { get; protected set; }

        protected RequestOperation() 
        {
            OnFinish += ReceiveResponse;
        }

        void ReceiveResponse()
        {
            InvokeOnReceiveResponse();
            var result = ResultCreation.Invoke();
            ProcessResult(result);
        }
        
        #region Invoke

        public void ProcessResult(T result)
        {
            if (IsSuccess())
                InvokeOnSuccess(result);
            else
                InvokeOnFail(result);
        }

        bool IsSuccess()
        {
            return SuccessCondition == null || SuccessCondition.Invoke();
        }

        public void InvokeOnReceiveResponse()
        {
            OnReceiveResponse?.Invoke();
            onReceiveResponseEvent?.Invoke();
        }

        public void InvokeOnSuccess(T result)
        {
            OnSuccess?.Invoke(result);
            onSuccessEvent?.Invoke(result);
        }
        
        public void InvokeOnFail(T result)
        {
            OnFail?.Invoke(result);
            onFailEvent?.Invoke(result);
        }
        
        #endregion

        #region Internal Action Bindings

        internal void SetOnFail(Action<T> callback)
        {
            OnFail = callback;
        }

        internal void SetOnSuccess(Action<T> callback)
        {
            OnSuccess = callback;
        }

        internal void SetOnReceiveResponse(Action callback)
        {
            OnReceiveResponse = callback;
        }

        #endregion

        #region Protected Result Creation & Success Condition

        internal void SetResultCreation(Func<T> creation)
        {
            ResultCreation = creation;
        }

        internal void SetSuccessCondition(Func<bool> successCondition)
        {
            SuccessCondition = successCondition;
        }

        #endregion
    }
    
}