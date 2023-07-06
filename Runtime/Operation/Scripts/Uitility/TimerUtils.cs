using System;

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
            => SetUpdateEvery(operation, TimeSpan.FromSeconds(secondsBetweenUpdate));

        public static Operation SetUpdateEveryFrame(this Operation operation)
        {
            operation.SetUpdateEveryFrame();
            return operation;
        }

        public static Operation SetUpdateEveryMoment(this Operation operation)
            => SetUpdateEvery(operation,TimeSpan.Zero);

        public static Operation SetUpdateEvery(this Operation operation, TimeSpan delay)
        {
            operation.SetUpdateEvery(delay);
            return operation;
        }
        
        public static Operation SetStartDelay(this Operation operation, float seconds)
            => SetStartDelay(operation, TimeSpan.FromSeconds(seconds));

        public static Operation SetStartDelay(this Operation operation, TimeSpan timeToDelay)
        {
            operation.SetStartDelay(timeToDelay);
            return operation;
        }

        public static Operation DelayStartToNextFrame(this Operation operation)
        {
            operation.DelayStartToNextFrame();
            return operation;
        }
        
        public static Operation ClearDelay(this Operation operation)
            => SetStartDelay(operation, TimeSpan.Zero);

        public static Operation SetExpireAfter(this Operation operation, float durationBeforeExpire, bool isUseRealTime)
            => SetExpireAfter(operation, TimeSpan.FromSeconds(durationBeforeExpire), isUseRealTime);

        public static Operation SetExpireAfter(this Operation operation, TimeSpan durationBeforeExpire, bool isUseRealTime)
        {
            var elapsedTime = isUseRealTime ? operation.ElapsedRealTime : TimeSpan.FromSeconds(operation.ElapsedDeltaTime);
            var expireTime = elapsedTime + durationBeforeExpire;
            return SetExpireIf(operation, () => (isUseRealTime ? operation.ElapsedRealTime : TimeSpan.FromSeconds(operation.ElapsedDeltaTime)) >= expireTime);
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