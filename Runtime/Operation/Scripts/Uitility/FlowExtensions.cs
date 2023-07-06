using System.Collections.Generic;

namespace PhEngine.Core.Operation
{
    public static class ChainedOperationExtensions
    {
        public static ChainedOperation ToRepeatedSeries (this Operation operation, int roundCount)
        {
            var operationList = new List<Operation>();
            for (int i = 0; i < roundCount; i++)
                operationList.Add(operation);
            
            return new ChainedOperation(operationList.ToArray());
        }
    }
}