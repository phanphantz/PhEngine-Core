using System;

namespace PhEngine.Core
{
    [Serializable]
    public abstract class State
    {
        public virtual string GetDisplayName() => GetType().Name;
    }
    
    [Serializable]
    public abstract class State<T> : State  where T : StateData
    {
        public T Data { get; protected set; }
        public event Action OnStart;
        
        public void Start(T data)
        {
            Data = data;
            OnStart?.Invoke();
            OnStarted(Data);
        }

        protected abstract void OnStarted(T data);
    }
    
    /// <summary>
    /// This type of state runs more than one frame
    /// </summary>
    public abstract class LongState<T> : State<T> where T : StateData
    {
        public event Action OnUpdate;
        public event Action OnEnd;
        public void Update(T data)
        {
            Data = data;
            OnUpdate?.Invoke();
            OnUpdated(data);
        }
        
        protected abstract void OnUpdated(T data);

        public void End(T data)
        {
            Data = data;
            OnEnd?.Invoke();
            OnEnded(Data);
        }

        protected abstract void OnEnded(T data);
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