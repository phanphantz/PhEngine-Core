using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PhEngine.Core
{
    [Serializable]
    public abstract class StateTracker
    {
        [SerializeField] string name;
        [SerializeField] bool isLogging;
        [SerializeField] float timeRate = 1f;
        
        protected float DeltaTime => Time.deltaTime * timeRate;

        protected void Log(string message)
        {
            if (!isLogging)
                return;
            
            Debug.Log($"[{name}] {message}");
        }
        
        protected void LogError(string message)
        {
            if (!isLogging)
                return;
            
            Debug.LogError($"[{name}] {message}");
        }
        
        public int StateCount => runningStateList.Count;
        
        protected StateProgress[] RunningStates => runningStateList.ToArray();
        [SerializeField] List<StateProgress> runningStateList = new List<StateProgress>();

        protected StateProgress FirstStateInList => runningStateList.FirstOrDefault();

        public event Action<State> OnStateStarted;
        public event Action<State> OnStateUpdated;
        public event Action<State> OnStateEnded;
        public event Action OnFinishedAll;

        public abstract void Update();
        public virtual StateProgress Append(State state)
        {
            var progress = new StateProgress(state);
            runningStateList.Add(progress);
            return progress;
        }

        protected void Execute(StateProgress progress)
        {
            var state = progress.State;
            if (state is IUnsafeState unsafeState && !unsafeState.CheckSafeToRun(out var error))
            {
                Debug.LogError(unsafeState.GetType().Name + " is not safe to run.\nReason:" + error);
                Remove(progress);
                return;
            }
            
            progress.Start(PrepareInfo(progress));
            OnStateStarted?.Invoke(state);
            Log("Executed: " + state.GetDisplayName());
        }

        protected StateData PrepareInfo(StateProgress progress)
        {
            var bareInfo = new StateData
            {
                tracker = this,
                elapsedTime = progress?.ElapsedTime ?? 0
            };
            return bareInfo;
        }
        
        protected bool TryEnd(StateProgress progress)
        {
            if (!progress.IsShouldEnd()) 
                return false;

            EndAndRemove(progress);
            return true;
        }

        protected void TryPassTimeAndUpdate(StateProgress progress)
        {
            progress.PassTime(DeltaTime);
            progress.Update(PrepareInfo(progress));
            OnStateUpdated?.Invoke(progress.State);
        }

        void EndAndRemove(StateProgress progress)
        {
            progress.End(PrepareInfo(progress));
            Log("End: " + progress.State.GetDisplayName());
            OnStateEnded?.Invoke(progress.State);
            Remove(progress);
        }

        void Remove(StateProgress progress)
        {
            runningStateList.Remove(progress);
            if (StateCount == 0)
                OnFinishedAll?.Invoke();
        }

        public void ForceEnd(State targetState)
        {
            var existingProgress = runningStateList.FirstOrDefault(s => s.State == targetState);
            if (existingProgress == null)
                Debug.LogError("Cannot end the target state since it was not found on the State Tracker");
            
            Log("Force End: " + targetState.GetDisplayName());
            EndAndRemove(existingProgress);
        }

        public bool TrySkip(State targetState)
        {
            if (targetState is not ISkippableState)
            {
                Debug.LogError(targetState.GetDisplayName() + "cannot be skipped.");
                return false;
            }
            
            if (targetState is IResolveBeforeSkip resolveBeforeSkip)
                resolveBeforeSkip.ResolveBeforeSkip();
            
            Log("Skipped: " + targetState.GetDisplayName());
            ForceEnd(targetState);
            return true;
        }

        public void DisposeAll(bool isEndBeforeRemove)
        {
            var runningStates = RunningStates;
            foreach (var progress in runningStates)
            {
                if (isEndBeforeRemove)
                    EndAndRemove(progress);
                else
                    Remove(progress);
            }
            runningStateList.Clear();
            Log("Disposed All States");
        }

        public bool HasStateOfType<T1>() where T1 : State
        {
            return runningStateList.Any(p => p.State.GetType() == typeof(T1));
        }
    }
}