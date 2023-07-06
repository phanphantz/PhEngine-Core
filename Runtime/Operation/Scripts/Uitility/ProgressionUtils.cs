using System;

namespace PhEngine.Core.Operation
{
    public static class ProgressionUtils
    {
        public static Operation DoUntil(this Operation operation, Func<bool> finishCondition)
            => SetProgressOn(operation,() => finishCondition.Invoke() ? 1f : 0f );
        
        public static Operation DoWhile(this Operation operation, Func<bool> blockingCondition)
            => SetProgressOn(operation,() => blockingCondition.Invoke() ? 0f : 1f );
        
        public static Operation AddProgressBy(this Operation operation, Func<float> progressAdder)
            => SetProgressOn(operation,() => operation.CurrentProgress + progressAdder.Invoke());

        public static Operation SetDuration(this Operation operation, float duration, bool isUseRealTime)
            => SetProgressOn(operation,() => GetProgressScaleFromTime(operation, TimeSpan.FromSeconds(duration), isUseRealTime));

        public static Operation SetDuration(this Operation operation, TimeSpan duration, bool isUseRealTime)
            => SetProgressOn(operation,() => GetProgressScaleFromTime(operation, duration, isUseRealTime));

        static float GetProgressScaleFromTime(Operation operation, TimeSpan duration, bool isUseRealTime)
            => (float)((isUseRealTime ? operation.ElapsedRealTime : TimeSpan.FromSeconds(operation.ElapsedDeltaTime)).TotalSeconds / duration.TotalSeconds);
        
        public static Operation ClearProgression(this Operation operation)
            => SetProgressOn(operation,null);

        public static Operation SetProgressOn(this Operation operation, Func<float> getter)
        {
            operation.SetProgressOn(getter);
            return operation;
        }
    }
}