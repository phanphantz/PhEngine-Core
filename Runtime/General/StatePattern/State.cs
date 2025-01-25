using System;

namespace PhEngine.Core
{
    [Serializable]
    public abstract class State
    {
        public StateData Data { get; internal set; }
        public virtual string GetDisplayName() => GetType().Name;

        public event Action OnStarted;
        
        public void Start(StateData data)
        {
            Data = data;
            OnStart(data);
            OnStarted?.Invoke();
        }

        protected abstract void OnStart(StateData data);
    }
    
    /// <summary>
    /// This type of state runs more than one frame
    /// </summary>
    public abstract class LongState : State
    {
        public event Action OnUpdated;
        public event Action OnEnded;
        public void Update(StateData data)
        {
            Data = data;
            OnUpdate(data);
            OnUpdated?.Invoke();
        }
        
        /// <summary>
        /// Logics to be executed frame by frame.
        /// OnUpdated is first called after the frame that this state is started.
        /// </summary>
        /// <param name="data"></param>
        protected abstract void OnUpdate(StateData data);

        public void End(StateData data)
        {
            Data = data;
            OnEnd(Data);
            OnEnded?.Invoke();
        }

        protected abstract void OnEnd(StateData data);
        public void ForceEnd()
        {
            Data.tracker.ForceEnd(this);
        }
    }
    
    /// <summary>
    /// This type of state ends immediately after execution
    /// </summary>
    public interface ISingleFrameState
    {
    }

    /// <summary>
    /// This type of state will end once reached a specific duration
    /// </summary>
    public interface IDurationBasedState
    {
        float Duration { get; }
    }

    /// <summary>
    /// This type of state will end once a specific condition is met
    /// </summary>
    public interface IConditionBasedState
    {
        bool IsShouldEnd { get; }
    }

    /// <summary>
    /// This type of state requires a condition check before execution
    /// </summary>
    public interface IUnsafeState
    {
        bool CheckSafeToRun(out Exception e);
    }

    /// <summary>
    /// This type of state can be skipped if requested
    /// </summary>
    public interface ISkippableState
    {
        
    }

    /// <summary>
    /// This type of state will execute a function before being skipped
    /// </summary>
    public interface IResolveBeforeSkip : ISkippableState
    {
        void ResolveBeforeSkip();
    }
}