using System;
using UnityEngine;

namespace PhEngine.Core
{
    [Serializable]
    public class StateProgress
    {
        public State State => state;
        [SerializeReference] State state;
        
        public float ElapsedTime => elapsedTime;
        [SerializeField] float elapsedTime;

        public bool IsStarted => isStarted;
        [SerializeField] bool isStarted;

        internal StateProgress(State state)
        {
            this.state = state;
        }

        internal void Start(StateData data)
        {
            isStarted = true;
            state.Start(data);
        }
        
        internal void PassTime(float deltaTime)
        {
            if (state is not LongState)
                return;
            
            elapsedTime += deltaTime;
        }

        internal void Update(StateData info)
        {
            if (state is not LongState longState)
                return;

            longState.Update(info);
        }

        public bool IsShouldEnd()
        {
            if (state is IDurationBasedState timeBasedState)
                return elapsedTime >= timeBasedState.Duration;

            if (state is IConditionBasedState conditionBasedState)
                return conditionBasedState.IsShouldEnd;

            return state is ISingleFrameState;
        }

        internal void End(StateData info)
        {
            if (state is LongState longState)
                longState.End(info);
        }
    }
}