using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PhEngine.Core
{
    [Serializable]
    public abstract class StateTracker
    {
        public abstract void ForceEnd(State targetState);
    }
    
    [Serializable]
    public abstract class StateTracker<T> : StateTracker where T : StateData
    {
        [SerializeField] float timeRate = 1f;
        
        StateProgress<T>[] RunningStates => runningStateList.ToArray();
        [SerializeField] List<StateProgress<T>> runningStateList = new List<StateProgress<T>>();
        
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

        void Update()
        {
            var deltaTime = Time.deltaTime * timeRate;
            var runningStates = RunningStates;
            foreach (var progress in runningStates)
            {
                progress.PassTime(deltaTime);
                progress.Update(PrepareInfo(progress));
            }

            foreach (var state in runningStates)
            {
                if (!state.IsShouldEnd()) 
                    continue;

                EndAndRemove(state);
            }
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