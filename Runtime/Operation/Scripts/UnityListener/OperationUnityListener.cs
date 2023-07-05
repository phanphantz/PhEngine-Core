using System;
using UnityEngine;
using UnityEngine.Events;

namespace PhEngine.Core.Operation
{
    public class OperationUnityListener : MonoBehaviour
    {
        public UnityEvent onStartEvent = new UnityEvent();
        public UnityEvent onUpdateEvent = new UnityEvent();
        public UnityEvent<float> onProgressEvent = new UnityEvent<float>();
        public UnityEvent<TimeSpan> onElapsedDeltaTimeEvent = new UnityEvent<TimeSpan>();
        public UnityEvent onFinishEvent = new UnityEvent();
        public UnityEvent onCancelEvent = new UnityEvent();
        public UnityEvent onPauseEvent = new UnityEvent();
        public UnityEvent onResumeEvent = new UnityEvent();

        public virtual void BindTo(Operation operation)
        {
            operation.OnStart += OnStartEvent;
            operation.OnUpdate += OnUpdateEvent;
            operation.OnProgress += OnProgressEvent;
            operation.OnElapsedDeltaTime += OnElapsedDeltaTimeEvent;
            operation.OnFinish += OnFinishEvent;
            operation.OnCancel +=  OnCancelEvent;
            operation.OnPause += OnPauseEvent;
            operation.OnResume += OnResumeEvent;
        }

        void OnResumeEvent()
        {
            onResumeEvent?.Invoke();
        }

        void OnPauseEvent()
        {
            onPauseEvent?.Invoke();
        }

        void OnCancelEvent()
        {
            onCancelEvent?.Invoke();
        }

        void OnFinishEvent()
        {
            onFinishEvent?.Invoke();
        }

        void OnElapsedDeltaTimeEvent(TimeSpan timeSpan)
        {
            onElapsedDeltaTimeEvent?.Invoke(timeSpan);
        }

        void OnProgressEvent(float progress)
        {
            onProgressEvent?.Invoke(progress);
        }

        void OnUpdateEvent()
        {
            onUpdateEvent?.Invoke();
        }

        void OnStartEvent()
        {
            onStartEvent?.Invoke();
        }

        public virtual void UnbindFrom(Operation operation)
        {
            operation.OnStart -= OnStartEvent;
            operation.OnUpdate -= OnUpdateEvent;
            operation.OnProgress -= OnProgressEvent;
            operation.OnElapsedDeltaTime -= OnElapsedDeltaTimeEvent;
            operation.OnFinish -= OnFinishEvent;
            operation.OnCancel -=  OnCancelEvent;
            operation.OnPause -= OnPauseEvent;
            operation.OnResume -= OnResumeEvent;
        }

        public static OperationUnityListener CreateFor(Operation operation)
        {
            var listener = Create();
            listener.BindTo(operation);
            return listener;
        }

        public static OperationUnityListener Create()
        {
            var obj = new GameObject {name = "OperationUnityListener"};
            return obj.AddComponent<OperationUnityListener>();
        }
    }
}