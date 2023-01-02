using System;
using UnityEngine;

namespace PhEngine.Core.Operation
{
    public static class OperationDelayer
    {
        public static Operation SetStartDelay(this Operation operation, float seconds)
            => SetStartDelay(operation, new WaitForSeconds(seconds));

        public static Operation SetStartDelay(this Operation operation, TimeSpan timeToDelay)
            => SetStartDelay(operation, new WaitForSeconds((float)timeToDelay.TotalSeconds));

        public static Operation DelayStartToNextFrame(this Operation operation)
            => SetStartDelay(operation, new WaitForEndOfFrame());
        
        public static Operation ClearDelay(this Operation operation)
            => SetStartDelay(operation, null);

        public static Operation SetStartDelay(this Operation operation, YieldInstruction delay)
        {
            operation.SetStartDelay(delay);
            return operation;
        }
    }
}