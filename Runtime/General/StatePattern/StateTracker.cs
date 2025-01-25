using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PhEngine.Core
{
    [Serializable]
    public abstract class StateTracker
    {
        [SerializeField] float timeRate = 1f;
        
        public abstract void ForceEnd(State targetState);
        protected float DeltaTime => Time.deltaTime * timeRate;
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
        
        protected abstract void Update();
        public void Execute(State<T> state)
        {
            var progress = new StateProgress<T>(state);
            progress.Start(PrepareInfo(progress));
            runningStateList.Add(progress);
        }
        
        protected T PrepareInfo(StateProgress<T> progress)
        {
            var bareInfo = CreateInfo();
            bareInfo.tracker = this;
            bareInfo.elapsedTime = progress?.ElapsedTime ?? 0;
            return bareInfo;
        }

        protected abstract T CreateInfo();

        protected bool TryEnd(StateProgress<T> state)
        {
            if (!state.IsShouldEnd()) 
                return false;

            EndAndRemove(state);
            return true;
        }

        protected void TryPassTimeAndUpdate(StateProgress<T> progress)
        {
            progress.PassTime(DeltaTime);
            progress.Update(PrepareInfo(progress));
        }

        void EndAndRemove(StateProgress<T> progress)
        {
            progress.End(PrepareInfo(progress));
            Remove(progress);
        }

        void Remove(StateProgress<T> progress)
        {
            runningStateList.Remove(progress);
        }

        public override void ForceEnd(State targetState)
        {
            var existingProgress = runningStateList.FirstOrDefault(s => s.State == targetState);
            if (existingProgress == null)
                Debug.LogError("Cannot end the target state since it was not found on the State Tracker");
            
            EndAndRemove(existingProgress);
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
        }

        public bool HasStateOfType<T1>() where T1 : State
        {
            return runningStateList.Any(p => p.State.GetType() == typeof(T1));
        }
    }
}