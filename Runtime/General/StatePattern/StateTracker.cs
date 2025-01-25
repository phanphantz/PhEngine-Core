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
        
        public abstract void ForceEnd(State targetState);
        public abstract bool TrySkip(State targetState);
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
    }

    [Serializable]
    public abstract class ParallelStateTracker<T> : StateTracker<T> where T : StateData
    {
        protected override void Update()
        {
            var runningStates = RunningStates;
            foreach (var progress in runningStates)
                TryPassTimeAndUpdate(progress);

            foreach (var state in runningStates)
                TryEnd(state);
        }
    }

    [Serializable]
    public abstract class QueuedStateTracker<T> : StateTracker<T> where T : StateData
    {
        protected override void Update()
        {
            if (StateCount == 0)
                return;

            var currentState = FirstStateInList;
            TryPassTimeAndUpdate(currentState);
            TryEnd(currentState);
        }
    }
    
    [Serializable]
    public abstract class StateTracker<T> : StateTracker where T : StateData
    {
        public int StateCount => runningStateList.Count;
        
        protected StateProgress<T>[] RunningStates => runningStateList.ToArray();
        [SerializeField] List<StateProgress<T>> runningStateList = new List<StateProgress<T>>();

        protected StateProgress<T> FirstStateInList => runningStateList.FirstOrDefault();

        public event Action<State<T>> OnStateStarted;
        public event Action<State<T>> OnStateUpdated;
        public event Action<State<T>> OnStateEnded;
        public event Action OnFinishedAll;
        
        protected abstract void Update();
        public void Execute(State<T> state)
        {
            if (state is IUnsafeState unsafeState && !unsafeState.CheckSafeToRun(out var error))
            {
                Debug.LogError(unsafeState.GetType().Name + " is not safe to run.\nReason:" + error);
                return;
            }
                
            var progress = new StateProgress<T>(state);
            progress.Start(PrepareInfo(progress));
            runningStateList.Add(progress);
            OnStateStarted?.Invoke(state);
            Log("Executed: " + state.GetDisplayName());
        }
        
        protected T PrepareInfo(StateProgress<T> progress)
        {
            var bareInfo = CreateInfo();
            bareInfo.tracker = this;
            bareInfo.elapsedTime = progress?.ElapsedTime ?? 0;
            return bareInfo;
        }

        protected abstract T CreateInfo();

        protected bool TryEnd(StateProgress<T> progress)
        {
            if (!progress.IsShouldEnd()) 
                return false;

            EndAndRemove(progress);
            OnStateEnded?.Invoke(progress.State);
            Log("End: " + progress.State.GetDisplayName());
            return true;
        }

        protected void TryPassTimeAndUpdate(StateProgress<T> progress)
        {
            progress.PassTime(DeltaTime);
            progress.Update(PrepareInfo(progress));
            OnStateUpdated?.Invoke(progress.State);
        }

        void EndAndRemove(StateProgress<T> progress)
        {
            progress.End(PrepareInfo(progress));
            Remove(progress);
        }

        void Remove(StateProgress<T> progress)
        {
            runningStateList.Remove(progress);
            if (StateCount == 0)
            {
                OnFinishedAll?.Invoke();
                Log("Finished All States");
            }
        }

        public override void ForceEnd(State targetState)
        {
            var existingProgress = runningStateList.FirstOrDefault(s => s.State == targetState);
            if (existingProgress == null)
                Debug.LogError("Cannot end the target state since it was not found on the State Tracker");
            
            EndAndRemove(existingProgress);
            Log("Force End: " + targetState.GetDisplayName());
        }

        public override bool TrySkip(State targetState)
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