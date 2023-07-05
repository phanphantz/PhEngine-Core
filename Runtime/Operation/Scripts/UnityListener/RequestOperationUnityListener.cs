using UnityEngine.Events;

namespace PhEngine.Core.Operation
{
    public abstract class RequestOperationUnityListener<T> : OperationUnityListener
    {
        public UnityEvent<T> onSuccessEvent = new UnityEvent<T>();
        public UnityEvent<T> onFailEvent = new UnityEvent<T>();
        public UnityEvent onReceiveResponseEvent = new UnityEvent();

        public override void BindTo(Operation operation)
        {
            base.BindTo(operation);
            if (operation is RequestOperation<T> requestOp)
            {
                requestOp.OnSuccess += OnSuccessEvent;
                requestOp.OnFail += OnFailEvent;
                requestOp.OnReceiveResponse += OnReceiveResponseEvent;
            }
        }

        void OnSuccessEvent(T result)
        {
            onSuccessEvent?.Invoke(result);
        }
        
        void OnFailEvent(T result)
        {
            onFailEvent?.Invoke(result);
        }
        
        void OnReceiveResponseEvent()
        {
            onReceiveResponseEvent?.Invoke();
        }

        public override void UnbindFrom(Operation operation)
        {
            base.UnbindFrom(operation);
            if (operation is RequestOperation<T> requestOp)
            {
                requestOp.OnSuccess -= OnSuccessEvent;
                requestOp.OnFail -= OnFailEvent;
                requestOp.OnReceiveResponse -= OnReceiveResponseEvent;
            }
        }
    }
}