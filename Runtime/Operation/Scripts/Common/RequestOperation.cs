using System;

namespace PhEngine.Core.Operation
{
    public interface IRequestOperation
    {
        internal void BindOnShotOnFailTypeless(Action callback);
        internal void BindOneShotOnSuccessTypeless(Action callback);
    }
    
    [Serializable]
    public class RequestOperation<T> : Operation, IRequestOperation
    {
        void IRequestOperation.BindOnShotOnFailTypeless(Action callback)
        {
            OnFail += CallOneTime;
            void CallOneTime(T val)
            {
                callback.Invoke();
                OnFail -= CallOneTime;
            }
        }

        void IRequestOperation.BindOneShotOnSuccessTypeless(Action callback)
        {
            OnSuccess += CallOneTime;
            void CallOneTime(T val)
            {
                callback.Invoke();
                OnSuccess -= CallOneTime;
            }
        }
        
        public event Action<T> OnSuccess;
        public event Action<T> OnFail;
        public event Action OnReceiveResponse;
        
        public event Func<bool> SuccessCondition;
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

        public virtual void ProcessResult(T result)
        {
            if (IsSuccess())
                InvokeOnSuccess(result);
            else
                InvokeOnFail(result);
        }

        protected override bool TryStopByGuardCondition()
        {
            var result = base.TryStopByGuardCondition();
            if (result)
                InvokeOnFail(GetGuardConditionResult());
            
            return result;
        }

        protected virtual T GetGuardConditionResult() => default;
        
        bool IsSuccess()
        {
            return SuccessCondition == null || SuccessCondition.Invoke();
        }

        public void InvokeOnReceiveResponse()
        {
            OnReceiveResponse?.Invoke();
        }

        public void InvokeOnSuccess(T result)
        {
            OnSuccess?.Invoke(result);
        }
        
        public void InvokeOnFail(T result)
        {
            OnFail?.Invoke(result);
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

        #region One-Shot Action Bindings

        internal void BindOneShotOnFail(Action<T> callback)
        {
            OnFail += Call;
            void Call(T value)
            {
                callback?.Invoke(value);
                OnFail -= Call;
            }
        }

        internal void BindOneShotOnSuccess(Action<T> callback)
        {
            OnSuccess  += Call;
            void Call(T value)
            {
                callback?.Invoke(value);
                OnSuccess -= Call;
            }
        }

        internal void BindOneShotOnReceiveResponse(Action callback)
        {
            OnReceiveResponse  += Call;
            void Call()
            {
                callback?.Invoke();
                OnReceiveResponse -= Call;
            }
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