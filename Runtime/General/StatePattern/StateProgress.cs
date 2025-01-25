using System;
using UnityEngine;

namespace PhEngine.Core
{
    [Serializable]
    public class StateProgress<T> where T : StateData
    {
        public State<T> State => state;
        [SerializeReference] State<T> state;
        
        public float ElapsedTime => elapsedTime;
        [SerializeField] float elapsedTime;

        internal StateProgress(State<T> state)
        {
            this.state = state;
        }

        internal void Start(T info)
        {
            state.Start(info);
        }
        
        internal void PassTime(float deltaTime)
        {
            if (state is not LongState<T>)
                return;
            
            elapsedTime += deltaTime;
        }

        internal void Update(T info)
        {
            if (state is not LongState<T> longState)
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

        internal void End(T info)
        {
            if (state is LongState<T> longState)
                longState.End(info);
        }
    }
}