using System;

namespace PhEngine.Core.Operation
{
    public static class OperationProgressionSetter
    {
        public static Operation DoUntil(this Operation operation, Func<bool> finishCondition)
            => SetProgressOn(operation,() => finishCondition.Invoke() ? 1f : 0f );
        
        public static Operation DoWhile(this Operation operation, Func<bool> blockingCondition)
            => SetProgressOn(operation,() => blockingCondition.Invoke() ? 0f : 1f );
        
        public static Operation AddProgressBy(this Operation operation, Func<float> progressAdder)
            => SetProgressOn(operation,() => operation.CurrentProgress + progressAdder.Invoke());

        public static Operation DoFor(this Operation operation, TimeSpan duration)
            => SetProgressOn(operation,() => GetProgressScaleFromTime(operation, duration));

        static float GetProgressScaleFromTime(Operation operation, TimeSpan duration)
            => (float)(operation.ElapsedTime.TotalSeconds / duration.TotalSeconds);
        
        public static Operation ClearProgression(this Operation operation)
            => SetProgressOn(operation,null);

        public static Operation SetProgressOn(this Operation operation, Func<float> getter)
        {
            operation.SetProgressOn(getter);
            return operation;
        }
        
        public static Operation SetAutoPauseIf(this Operation operation, Func<bool> autoPauseCondition)
        {
            operation.SetAutoPauseIf(autoPauseCondition);
            return operation;
        }
        
        public static Operation ClearAutoPause(this Operation operation)
            => SetAutoPauseIf(operation,null);
        
        public static Operation SetAutoResumeIf(this Operation operation, Func<bool> autoResumeCondition)
        {
            operation.SetAutoResumeIf(autoResumeCondition);
            return operation;
        }
        
        public static Operation ClearAutoResume(this Operation operation)
            => SetAutoResumeIf(operation,null);
    }
}