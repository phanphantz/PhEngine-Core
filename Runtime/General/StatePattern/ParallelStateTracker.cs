using System;

namespace PhEngine.Core
{
    [Serializable]
    public class ParallelStateTracker : StateTracker
    {
        public override void Update()
        {
            var runningStates = RunningStates;
            foreach (var progress in runningStates)
                TryPassTimeAndUpdate(progress);

            foreach (var state in runningStates)
                TryEnd(state);
        }

        public override StateProgress Append(State state)
        {
            var progress = base.Append(state);
            Execute(progress);
            return progress;
        }
    }
}