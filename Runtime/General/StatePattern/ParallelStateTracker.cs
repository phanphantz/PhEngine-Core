using System;

namespace PhEngine.Core
{
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

        public override StateProgress<T> Append(State<T> state)
        {
            var progress = base.Append(state);
            Execute(progress);
            return progress;
        }
    }
}