using System;

#if UNITASK
using Cysharp.Threading.Tasks;
#endif

namespace PhEngine.Core.Operation
{
    public interface IRequestOperation
    {
        internal void BindOnFailTypeless(Action callback, bool isOneShot = false);
        internal void BindOnSuccessTypeless(Action callback, bool isOneShot = false);
    }

    [Serializable]
    public class RequestOperation<T> : Operation, IRequestOperation
    {
        void IRequestOperation.BindOnFailTypeless(Action callback, bool isOneShot)
        {
            OnFail += CallOneTime;
            void CallOneTime(T val)
            {
                callback.Invoke();
                if (isOneShot)
                    OnFail -= CallOneTime;
            }
        }

        void IRequestOperation.BindOnSuccessTypeless(Action callback, bool isOneShot)
        {
            OnSuccess += CallOneTime;
            void CallOneTime(T val)
            {
                callback.Invoke();
                if (isOneShot)
                    OnSuccess -= CallOneTime;
            }
        }
        
        public event Action<T> OnSuccess;
        public event Action<T> OnFail;
        public event Action OnReceiveResponse;
        
        public event Func<bool> SuccessCondition;
        public Func<T> ResultCreation { get; protected set; }

        bool wasSuccess;

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
            wasSuccess = IsSuccess();
            if (wasSuccess)
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

#if UNITASK
        protected override async UniTask PostProcessTask()
        {
            await base.PostProcessTask();
            if (!wasSuccess)
                throw new OperationCanceledException();
        }
#endif
        
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
        
        #region Action Bindings

        protected void BindOnFailInternally(Action<T> callback, bool isOneShot = false)
        {
            OnFail += Call;
            void Call(T value)
            {
                callback?.Invoke(value);
                if (isOneShot)
                    OnFail -= Call;
            }
        }

        protected void BindOnSuccessInternally(Action<T> callback, bool isOneShot = false)
        {
            OnSuccess  += Call;
            void Call(T value)
            {
                callback?.Invoke(value);
                if (isOneShot)
                    OnSuccess -= Call;
            }
        }

        protected void BindOnReceiveResponseInternally(Action callback, bool isOneShot = false)
        {
            OnReceiveResponse  += Call;
            void Call()
            {
                callback?.Invoke();
                if (isOneShot)
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