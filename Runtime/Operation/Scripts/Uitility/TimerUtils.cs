using System;
using UnityEngine;

namespace PhEngine.Core.Operation
{
    public static class TimerUtils
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
        
        public static Operation SetExpireAfter(this Operation operation, TimeSpan durationBeforeExpire)
        {
            var expireTime = operation.ElapsedTime + durationBeforeExpire;
            return SetExpireIf(operation, () => operation.ElapsedTime >= expireTime);
        }

        public static Operation ClearExpiration(this Operation operation)
            => SetExpireIf(operation, null);
        
        public static Operation SetExpireIf(this Operation operation, Func<bool> expireCondition)
        {
            operation.SetExpireIf(expireCondition);
            return operation; 
        }
    }
}