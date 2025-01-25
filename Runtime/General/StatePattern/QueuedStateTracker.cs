using System;

namespace PhEngine.Core
{
    [Serializable]
    public class QueuedStateTracker : StateTracker
    {
        protected override void Update()
        {
            if (StateCount == 0)
                return;

            var currentState = FirstStateInList;
            if (!currentState.IsStarted)
            {
                Execute(currentState);
                return;
            }
            TryPassTimeAndUpdate(currentState);
            TryEnd(currentState);
        }
    }
}