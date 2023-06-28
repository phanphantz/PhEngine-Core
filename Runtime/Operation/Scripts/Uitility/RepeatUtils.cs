using System;

namespace PhEngine.Core.Operation
{
    public static class RepeatUtils
    {
        public static Operation SetRepeatForever(this Operation operation)
            => SetRepeatIf(operation, () => true);

        public static Operation SetRepeatFor(this Operation operation, int repeatCount)
            => SetRepeatIf(operation,() => IsShouldRepeatBecauseRepeatQuota(operation, repeatCount));

        static bool IsShouldRepeatBecauseRepeatQuota(Operation operation, int repeatCount)
        {
            return repeatCount > 0 
                   && operation.CurrentRound < 1 + repeatCount;
        }

        public static Operation SetRepeatUntil(this Operation operation, Func<bool> stopCondition)
            => SetRepeatIf(operation,()=> stopCondition.Invoke() == false);
        
        public static Operation ClearRepeat(this Operation operation)
            => SetRepeatIf(operation,null);

        public static Operation SetRepeatIf(this Operation operation, Func<bool> repeatCondition)
        {
            operation.SetRepeatIf(repeatCondition);
            return operation;
        }
    }
}