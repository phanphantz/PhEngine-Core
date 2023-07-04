using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

#if UNITY_EDITOR && EDITOR_COROUTINE
using Unity.EditorCoroutines.Editor;
#endif

namespace PhEngine.Core.Operation
{
    [Serializable]
    public class Operation
    {
        public static DateTimeFormat DateTimeFormat { get; set; } = DateTimeFormat.UTC;
        protected MonoBehaviour Host => host;
        MonoBehaviour host;
        Coroutine activeRoutine;
        
#if UNITY_EDITOR && EDITOR_COROUTINE
        EditorCoroutine activeEditorRoutine;
#endif
        public float CurrentProgress { get; private set; }
        public TimeSpan ElapsedDeltaTime { get; private set; }
        public TimeSpan ElapsedRealTime
        {
            get
            {
                if (!startTimeFromStartup.HasValue)
                    return TimeSpan.Zero;

                if (endTimeFromStartup.HasValue)
                    return TimeSpan.FromSeconds(endTimeFromStartup.Value - startTimeFromStartup.Value);
                
                return TimeSpan.FromSeconds(Time.realtimeSinceStartup - startTimeFromStartup.Value);
            }
        }
        
        public DateTime? StartTime { get; private set; }
        public DateTime? EndTime { get; private set; }
        public int CurrentRound { get; private set; }
        public float TimeScale { get; private set; } = 1f;

        public bool IsActive => IsStarted && !IsFinished;
        public bool IsStarted { get; private set; }
        public bool IsFinished { get; private set; }
        public bool IsPaused { get; private set; }

        public event Action<float> OnProgress;
        public event Action<TimeSpan> OnElapsedDeltaTime;
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
        public UnityEvent<TimeSpan> onElapsedDeltaTimeEvent = new UnityEvent<TimeSpan>();
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
        public event Func<bool> GuardCondition;

        float? startTimeFromStartup;
        float? endTimeFromStartup;

        bool isRunningAsExternalCoroutine;

        public Operation(Operation operation)
        {
      
        }

        #region Constructors

        public Operation()
        {
            TreatAsAction();
        }
        
        public Operation(Action action)
        {
            OnStart += action;
            TreatAsAction();
        }
        
        void TreatAsAction()
        {
            ProgressGetter = () => 1f;
        }
        
        #endregion

        #region Control Methods

        public virtual void RunOn(MonoBehaviour target)
        {
            if (!TryStart())
                return;

            host = target;
            var routine = ProcessRoutine(CurrentRound);
            if (Application.isPlaying && host)
            {
                activeRoutine = host.StartCoroutine(routine);
            }
            else
            {
#if UNITY_EDITOR && EDITOR_COROUTINE
                if (host == null)
                    EditorCoroutineUtility.StartCoroutineOwnerless(routine);
                else
                    activeEditorRoutine = EditorCoroutineUtility.StartCoroutine(routine, host);
#else
                throw new ArgumentNullException(nameof(target));
#endif
            }
        }

        bool TryStart()
        {
            isRunningAsExternalCoroutine = false;
            if (IsActive)
            {
                Debug.LogWarning("Cannot run operation that is already started.");
                return false;
            }

            if (TryStopByGuardCondition()) 
                return false;

            ResetProgress();
            IsFinished = false;
            IsStarted = true;
            StartTime = GetCurrentDeviceTime();
            startTimeFromStartup = Time.realtimeSinceStartup;
            endTimeFromStartup = null;
            EndTime = null;
            CurrentRound++;
            return true;
        }

        protected virtual bool TryStopByGuardCondition()
        {
            if (GuardCondition != null && GuardCondition.Invoke())
            {
                Debug.Log("Guard Condition detected, an operation will not run.");
                return true;
            }

            return false;
        }

        public void RestartOn(MonoBehaviour target)
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

        public IEnumerator Coroutine()
        {
            while (IsShouldRepeat() || !isRunningAsExternalCoroutine)
            {
                if (!TryStart())
                    yield break;

                isRunningAsExternalCoroutine = true;
                yield return ProcessRoutine(CurrentRound);
            }
            CurrentRound = 0;
        }
        
        IEnumerator ProcessRoutine(int assignedRound)
        {
            yield return StartDelay;
            InvokeOnStart();
            
            if (TryFinishOrKill(assignedRound))
                yield break;
            
            while (!TryFinishOrKill(assignedRound))
                yield return UpdateRoutine(assignedRound);
        }
        
        void ResetProgress()
        {
            ElapsedDeltaTime = TimeSpan.Zero;
            ForceSetProgress(0);
        }
        
        void ForceSetProgress(float progress)
        {
            CurrentProgress = progress;
            InvokeOnProgress(progress);
        }
        
        static DateTime GetCurrentDeviceTime()
        {
            return DateTimeFormat == DateTimeFormat.UTC ? DateTime.UtcNow : DateTime.Now;
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
            
            if (!IsActive)
                return true;

            ForceFinish();
            return true;
        }

        protected virtual bool IsShouldFinish()
        {
            return CurrentProgress >= 1f;
        }

        protected void ForceFinish()
        {
            SetProgress(1f);
            IsFinished = true;
            EndTime = GetCurrentDeviceTime();
            endTimeFromStartup = Time.realtimeSinceStartup;
            InvokeOnFinish();
            TryRepeatIfWasRunByHost();
        }

        void TryRepeatIfWasRunByHost()
        {
            if (isRunningAsExternalCoroutine)
                return;

            if (IsShouldRepeat())
                RunOn(Host);
            else
                CurrentRound = 0;
        }

        public bool IsShouldRepeat()
        {
            return RepeatCondition != null && RepeatCondition.Invoke();
        }

        void SetProgress(float progress)
        {
            if (Mathf.Approximately(CurrentProgress, progress))
                return;

            ForceSetProgress(progress);
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
            
            void PassTimeByDeltaTime()
            {
                var passedTime = Time.deltaTime * TimeScale;
                ElapsedDeltaTime += TimeSpan.FromSeconds(passedTime);
                InvokeOnDeltaTimeChange();
            }

            void RefreshProgress()
            {
                if (ProgressGetter != null)
                    SetProgress(ProgressGetter.Invoke());
            }
            
            bool IsShouldNotAutoResume()
            {
                return AutoResumeCondition == null || !AutoResumeCondition.Invoke();
            }
        
            bool IsShouldAutoPause()
            {
                return AutoPauseCondition != null && AutoPauseCondition.Invoke();
            }
        }

        protected virtual void ForceCancel()
        {
#if UNITY_EDITOR && EDITOR_COROUTINE
            if (activeEditorRoutine != null)
                EditorCoroutineUtility.StopCoroutine(activeEditorRoutine);
            
            activeEditorRoutine = null;
#endif
            if (host)
            {
                if (activeRoutine != null)
                    host.StopCoroutine(activeRoutine);
            
                activeRoutine = null;
                host = null;
            }
            
            StartTime = null;
            startTimeFromStartup = null;
            endTimeFromStartup = null;
            EndTime = null;
            IsStarted = false;
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
        
        public void InvokeOnDeltaTimeChange()
        {
            OnElapsedDeltaTime?.Invoke(ElapsedDeltaTime);
            onElapsedDeltaTimeEvent?.Invoke(ElapsedDeltaTime);
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
            OnElapsedDeltaTime = callback;
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

        #region Static Helper Functions

        public static Operation Create()
        {
            return new Operation();
        }

        public static Operation From(Action action)
        {
            return new Operation(action);
        }
        
        #endregion
    }

    public enum DateTimeFormat
    {
        UTC, Local
    }
}