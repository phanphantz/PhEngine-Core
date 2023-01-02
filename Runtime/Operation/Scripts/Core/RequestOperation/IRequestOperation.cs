using System;

namespace PhEngine.Core.Operation
{
    public interface IRequestOperation
    {
        internal void AppendOnFail(Action callback);
        internal void AppendOnSuccess(Action callback);
    }
}