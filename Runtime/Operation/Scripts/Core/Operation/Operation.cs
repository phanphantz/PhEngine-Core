using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace PhEngine.Core.Operation
{
    [Serializable]
    public class Operation
    {
        protected MonoBehaviour Host => host;
        MonoBehaviour host;
        Coroutine activeRoutine;
        
        public float CurrentProgress { get; private set; }
        public TimeSpan ElapsedTime { get; private set; }
        public int CurrentRound { get; private set; }
        public float TimeScale { get; private set; } = 1f;
        
        public bool IsActive => IsStarted && !IsFinished;
        public bool IsStarted { get; private set; }
        public bool IsFinished { get; private set; }
        public bool IsPaused { get; private set; }
        
        public event Action<float> OnProgress;
        public event Action<TimeSpan> OnTimeChange;
        public event Action OnStart;
        public event Action OnUpdate;
        public event Action OnPause;
        public event Action OnResume;
        public event Action OnFinish;
        public event Action OnCancel;

        [Header("Unity Events")] 
        public UnityEvent onStartEvent = new UnityEvent();
        public UnityEvent onUpdateEvent = new UnityEvent();
        public UnityEvent<float> onProgressEvent = new UnityEvent<float>();
        public UnityEvent<TimeSpan> onTimeChangeEvent = new UnityEvent<TimeSpan>();
        public UnityEvent onFinishEvent = new UnityEvent();
        public UnityEvent onCancelEvent = new UnityEvent();
        public UnityEvent onPauseEvent = new UnityEvent();
        public UnityEvent onResumeEvent = new UnityEvent();

        public YieldInstruction StartDelay { get; protected set; }
        public YieldInstruction UpdateDelay { get; protected set; }
        
        public Func<float> ProgressGetter { get; protected set; }
        public Func<bool> ExpireCondition { get; protected set; }
        public Func<bool> RepeatCondition { get; protected set; }
        public Func<bool> AutoPauseCondition { get; protected set; }
        public Func<bool> AutoResumeCondition { get; protected set; }

        #region Control Methods

        public void RunOn(MonoBehaviour target)
        {
            if (IsActive)
            {
                Debug.LogWarning("Cannot run operation that is already started.");
                return;
            }
            
            ForceRunOn(target);
        }

        public void RunOnIfNotActive(MonoBehaviour target)
        {
            if (IsActive)
                return;
            
            ForceRunOn(target);
        }
        
        public void Restart(MonoBehaviour target)
        {
            if (IsActive)
                Cancel();
            
            RunOn(target);
        }

        public void Cancel()
        {
            if (!IsActive)
            {
                Debug.LogWarning("Cannot cancel operation that is not active.");
                return;
            }
            
            ForceCancel();
            InvokeOnCancel();
        }
        
        public void Finish()
        {
            if (!IsActive)
            {
                Debug.LogWarning("Cannot force finish operation that is not active.");
                return;
            }

            IsPaused = false;
            ForceFinish();
        }

        public void Pause()
        {
            if (!IsActive)
            {
                Debug.LogWarning("Cannot pause operation that is not active.");
                return;
            }

            if (IsPaused)
                return;
            
            IsPaused = true;
            InvokeOnPause();
        }
        
        public void Resume()
        {
            if (!IsActive)
            {
                Debug.LogWarning("Cannot resume operation that is not active.");
                return;
            }

            if (!IsPaused)
                return;
            
            IsPaused = false;
            InvokeOnResume();
        }
        
        #endregion

        #region Work Cycle Logic

        void ForceRunOn(MonoBehaviour target)
        {
            host = target;
            StartNewRound();
        }

        void StartNewRound()
        {
            ResetProgress();
            IsFinished = false;
            IsStarted = true;
            CurrentRound++;
            RunOnHost();
        }
        
        void ResetProgress()
        {
            ElapsedTime = TimeSpan.Zero;
            ForceSetProgress(0);
        }
        
        void ForceSetProgress(float progress)
        {
            CurrentProgress = progress;
            InvokeOnProgress(progress);
        }

        void RunOnHost()
        {
            var routine = OperationRoutine(CurrentRound);
            activeRoutine = host.StartCoroutine(routine);
        }

        IEnumerator OperationRoutine(int assignedRound)
        {
            yield return StartDelay;
            InvokeOnStart();
            if (TryFinishOrKill(assignedRound))
                yield break;
            
            while (!TryFinishOrKill(assignedRound))
                yield return UpdateRoutine(assignedRound);
        }

        bool TryFinishOrKill(int round)
        {
            if (CurrentRound != round)
                return true;

            if (ExpireCondition != null && ExpireCondition.Invoke())
            {
                ForceCancel();
                return true;
            }

            if (!IsShouldFinish()) 
                return false;
            
            TryFinish();
            return true;
        }

        protected virtual bool IsShouldFinish()
        {
            return CurrentProgress >= 1f;
        }

        void TryFinish()
        {
            if (!IsActive)
                return;

            ForceFinish();
        }
        
        protected void ForceFinish()
        {
            SetProgress(1f);
            IsFinished = true;
            InvokeOnFinish();
            TryRepeatAfterFinish();
        }

        void SetProgress(float progress)
        {
            if (Mathf.Approximately(CurrentProgress, progress))
                return;

            ForceSetProgress(progress);
        }

        void TryRepeatAfterFinish()
        {
            if (RepeatCondition == null)
                return;
            
            if (RepeatCondition.Invoke())
                StartNewRound();
        }
        
        protected virtual IEnumerator UpdateRoutine(int assignRound)
        {
            if (IsPaused)
            {
                if (IsShouldNotAutoResume())
                    yield break;
             
                Resume();
                if (TryFinishOrKill(assignRound))
                    yield break;
            }
            else
            {
                if (IsShouldAutoPause())
                {
                    Pause();
                    yield break;
                }
            }
            
            PassTimeByDeltaTime();
            if (TryFinishOrKill(assignRound))
                yield break;
            
            InvokeOnUpdate();
            if (TryFinishOrKill(assignRound))
                yield break;
            
            RefreshProgress();
            if (TryFinishOrKill(assignRound))
                yield break;
            
            yield return UpdateDelay;
        }
        
        bool IsShouldNotAutoResume()
        {
            return AutoResumeCondition == null || !AutoResumeCondition.Invoke();
        }
        
        bool IsShouldAutoPause()
        {
            return AutoPauseCondition != null && AutoPauseCondition.Invoke();
        }

        void PassTimeByDeltaTime()
        {
            var passedTime = Time.deltaTime * TimeScale;
            ElapsedTime += TimeSpan.FromSeconds(passedTime);
            InvokeOnTimeChange();
        }

        void RefreshProgress()
        {
            if (ProgressGetter != null)
                SetProgress(ProgressGetter.Invoke());
        }

        protected virtual void ForceCancel()
        {
            CancelRunOnHost();
            ResetProgress();
            IsStarted = false;
        }
        
        protected void CancelRunOnHost()
        {
            if (host == null)
                return;

            if (activeRoutine == null)
                return;

            host.StopCoroutine(activeRoutine);
            activeRoutine = null;
            host = null;
        }
        
        #endregion

        #region Invoke
        
        public void InvokeOnStart()
        {
            OnStart?.Invoke();
            onStartEvent?.Invoke();
        }
        
        public void InvokeOnUpdate()
        {
            OnUpdate?.Invoke();
            onUpdateEvent?.Invoke();
        }
        
        public void InvokeOnTimeChange()
        {
            OnTimeChange?.Invoke(ElapsedTime);
            onTimeChangeEvent?.Invoke(ElapsedTime);
        }
        
        public void InvokeOnProgress(float progress)
        {
            OnProgress?.Invoke(progress);
            onProgressEvent?.Invoke(progress);
        }
        
        public void InvokeOnFinish()
        {
            OnFinish?.Invoke();
            onFinishEvent?.Invoke();
        }

        public void InvokeOnCancel()
        {
            OnCancel?.Invoke();
            onCancelEvent?.Invoke();
        }
        
        public void InvokeOnPause()
        {
            OnPause?.Invoke();
            onPauseEvent?.Invoke();
        }

        public void InvokeOnResume()
        {
            OnResume?.Invoke();
            onResumeEvent?.Invoke();
        }
        
        #endregion

        #region Internal Action Bindings

        internal void SetOnStart(Action callback)
        {
            OnStart = callback;
        }
        
        internal void SetOnUpdate(Action callback)
        {
            OnUpdate = callback;
        }
        
        internal void SetOnTimeChange(Action<TimeSpan> callback)
        {
            OnTimeChange = callback;
        }

        internal void SetOnProgress(Action<float> callback)
        {
            OnProgress = callback;
        }
        
        internal void SetOnFinish(Action callback)
        {
            OnFinish = callback;
        }
        
        internal void SetOnCancel(Action callback)
        {
            OnCancel = callback;
        }
        
        internal void SetOnResume(Action callback)
        {
            OnResume = callback;
        }

        internal void SetOnPause(Action callback)
        {
            OnPause = callback;
        }

        #endregion

        #region Internal Update & Progression

        internal void SetUpdateEvery(YieldInstruction value)
            => UpdateDelay = value;

        internal void SetProgressOn(Func<float> getter)
            => ProgressGetter = getter;

        #endregion

        #region Internal Behaviour Setter
        
        internal void SetTimeScale(float value)
            => TimeScale = value;

        internal void SetStartDelay(YieldInstruction value) 
            => StartDelay = value;
        
        internal void SetRepeatIf(Func<bool> repeatCondition)
            => RepeatCondition = repeatCondition;
        
        internal void SetExpireIf(Func<bool> expireCondition)
            => ExpireCondition = expireCondition;
        
        internal void SetAutoPauseIf(Func<bool> autoPauseCondition)
            => AutoPauseCondition = autoPauseCondition;

        internal void SetAutoResumeIf(Func<bool> autoResumeCondition)
            => AutoResumeCondition = autoResumeCondition;

        #endregion

        public static Operation Do()
        {
            return new Operation();
        }
    }
}