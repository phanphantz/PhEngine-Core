using System;
using UnityEngine;

namespace PhEngine.Core.Operation
{
    public static class OperationTimer
    {
        public static Operation SetTimeScale(this Operation operation, float timeScale)
        {
            operation.SetTimeScale(timeScale);
            return operation;
        }

        public static Operation ResetTimeScale(this Operation operation)
            => SetTimeScale(operation, 1f);

        public static Operation SetUpdateEvery(this Operation operation, float secondsBetweenUpdate)
            => SetUpdateEvery(operation, new WaitForSeconds(secondsBetweenUpdate));

        public static Operation SetUpdateEvery(this Operation operation, TimeSpan timeBetweenUpdate)
            => SetUpdateEvery(operation,new WaitForSeconds((float)timeBetweenUpdate.TotalSeconds));

        public static Operation SetUpdateEveryFrame(this Operation operation)
            => SetUpdateEvery(operation,new WaitForEndOfFrame());

        public static Operation SetUpdateEveryMoment(this Operation operation)
            => SetUpdateEvery(operation,null);

        public static Operation SetUpdateEvery(this Operation operation, YieldInstruction delay)
        {
            operation.SetUpdateEvery(delay);
            return operation;
        }
    }
}