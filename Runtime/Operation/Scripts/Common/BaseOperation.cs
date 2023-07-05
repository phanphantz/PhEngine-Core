using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace PhEngine.Core.Operation
{
    public abstract class BaseOperation
    {
        //Status
        public bool IsStarted { get; private set; }
        public bool IsFinished { get; private set; }
        public bool IsPaused { get; private set; }
        
        //Actions
        public event Action<float> OnProgress;
        public event Action<TimeSpan> OnElapsedDeltaTime;
        public event Action OnStart;
        public event Action OnUpdate;
        public event Action OnPause;
        public event Action OnResume;
        public event Action OnFinish;
        public event Action OnCancel;
        
        //Behaviors
        public Func<float> ProgressGetter { get; protected set; }
        public Func<bool> ExpireCondition { get; protected set; }
        public Func<bool> RepeatCondition { get; protected set; }
        public event Func<bool> GuardCondition;
        public Flow ParentFlow { get; private set; }

        //Time
        public static DateTimeFormat DateTimeFormat { get; set; } = DateTimeFormat.UTC;
        public float CurrentProgress { get; private set; }
        public TimeSpan ElapsedDeltaTime { get; private set; }
        public DateTime? StartTime { get; private set; }
        public DateTime? EndTime { get; private set; }
        public int CurrentRound { get; protected set; }
        public float TimeScale { get; private set; } = 1f;
        public CustomYieldInstruction StartDelay { get; protected set; }
        public CustomYieldInstruction UpdateDelay { get; protected set; }
        
        #region Getters
        
        public bool IsActive => IsStarted && !IsFinished;
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
        
        protected virtual bool IsShouldFinish => CurrentProgress >= 1f;
        public bool IsShouldRepeat() => RepeatCondition != null && RepeatCondition.Invoke();
        static DateTime GetCurrentDeviceTime() => DateTimeFormat == DateTimeFormat.UTC ? DateTime.UtcNow : DateTime.Now;
        
        #endregion

        float? startTimeFromStartup;
        float? endTimeFromStartup;

        protected BaseOperation()
        {
            TreatAsAction();
        }

        protected BaseOperation(Action action)
        {
            OnStart += action;
            TreatAsAction();
        }
        
        void TreatAsAction()
        {
            ProgressGetter = () => 1f;
        }

        #region Control Methods

        public void Restart()
        {
            if (IsActive)
                Cancel();
            
            RunInternally();
        }
        
        public void Cancel()
        {
            if (!IsActive)
            {
                Debug.LogWarning("Cannot cancel operation that is not active.");
                return;
            }
            
            NotifyCancel();
            InvokeOnCancel();
        }
        
        protected abstract void RunInternally();

        public void Finish()
        {
            if (!IsActive)
            {
                Debug.LogWarning("Cannot force finish operation that is not active.");
                return;
            }

            IsPaused = false;
            NotifyFinishAndTryRepeat();
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

        #region Internal Control Methods
        
        protected virtual bool TryStart()
        {
            if (IsActive)
            {
                Debug.LogWarning("Cannot run operation that is already started.");
                return false;
            }

            if (TryStopByGuardCondition()) 
                return false;

            NotifyStart();
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
        
        protected virtual void NotifyStart()
        {
            ResetProgress();
            IsFinished = false;
            IsStarted = true;
            StartTime = GetCurrentDeviceTime();
            startTimeFromStartup = Time.realtimeSinceStartup;
            endTimeFromStartup = null;
            EndTime = null;
            CurrentRound++;
        }
        
        protected virtual void NotifyCancel()
        {
            StartTime = null;
            startTimeFromStartup = null;
            endTimeFromStartup = null;
            EndTime = null;
            IsStarted = false;
        }
        
        protected virtual void NotifyFinishAndTryRepeat()
        {
            NotifyFinish();
            TryRepeat();
        }

        protected void NotifyFinish()
        {
            SetProgress(1f);
            IsFinished = true;
            EndTime = GetCurrentDeviceTime();
            endTimeFromStartup = Time.realtimeSinceStartup;
            InvokeOnFinish();
        }

        protected virtual void TryRepeat()
        {
            if (IsShouldRepeat())
                RunInternally();
            else
                CurrentRound = 0;
        }
        
        protected virtual bool TryFinishOrKill(int round)
        {
            var status = GetRunningStatus(round);
            switch (status)
            {
                case Ending.Finish:
                    NotifyFinishAndTryRepeat();
                    return true;
                case Ending.Cancel:
                    NotifyCancel();
                    return true;
                default:
                    return false;
            }
        }

        protected Ending GetRunningStatus(int round)
        {
            if (CurrentRound != round)
                return Ending.Finish;

            if (ExpireCondition != null && ExpireCondition.Invoke())
                return Ending.Cancel;

            return !IsShouldFinish ? Ending.NotReached : Ending.Finish;
        }

        protected enum Ending
        {
            NotReached, Finish, Cancel
        }
        
        #endregion

        #region Progression Methods
        
        void SetProgress(float progress)
        {
            if (Mathf.Approximately(CurrentProgress, progress))
                return;

            ForceSetProgress(progress);
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
        
        protected void PassTimeByDeltaTime()
        {
            var passedTime = Time.deltaTime * TimeScale;
            ElapsedDeltaTime += TimeSpan.FromSeconds(passedTime);
            InvokeOnDeltaTimeChange();
        }

        protected void RefreshProgress()
        {
            if (ProgressGetter != null)
                SetProgress(ProgressGetter.Invoke());
        }
        
        #endregion
        
        #region Invoke
        
        public void InvokeOnStart()
        {
            OnStart?.Invoke();
        }
        
        public void InvokeOnUpdate()
        {
            OnUpdate?.Invoke();
        }
        
        public void InvokeOnDeltaTimeChange()
        {
            OnElapsedDeltaTime?.Invoke(ElapsedDeltaTime);
        }
        
        public void InvokeOnProgress(float progress)
        {
            OnProgress?.Invoke(progress);
        }
        
        public void InvokeOnFinish()
        {
            OnFinish?.Invoke();
        }

        public void InvokeOnCancel()
        {
            OnCancel?.Invoke();
        }
        
        public void InvokeOnPause()
        {
            OnPause?.Invoke();
        }

        public void InvokeOnResume()
        {
            OnResume?.Invoke();
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
        
        #region Internal Behaviour Setter
        
        internal void SetTimeScale(float value)
            => TimeScale = value;

        internal void SetStartDelay(CustomYieldInstruction value) 
            => StartDelay = value;
        
        internal void SetRepeatIf(Func<bool> repeatCondition)
            => RepeatCondition = repeatCondition;
        
        internal void SetExpireIf(Func<bool> expireCondition)
            => ExpireCondition = expireCondition;
        
        internal void SetParentFlow(Flow flow)
            => ParentFlow = flow;

        #endregion
        
        #region Internal Update & Progression

        internal void SetUpdateEvery(CustomYieldInstruction value)
            => UpdateDelay = value;

        internal void SetProgressOn(Func<float> getter)
            => ProgressGetter = getter;

        #endregion
    }
    
    public enum DateTimeFormat
    {
        UTC, Local
    }
}