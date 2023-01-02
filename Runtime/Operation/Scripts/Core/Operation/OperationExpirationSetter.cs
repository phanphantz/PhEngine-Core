using System;

namespace PhEngine.Core.Operation
{
    public static class OperationExpirationSetter
    {
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