using System;
using UnityEngine.Networking;

#if UNITASK
using Cysharp.Threading.Tasks;
#endif

namespace PhEngine.Core.Operation
{
    [Serializable]
    public abstract class NetworkOperation<T> : RequestOperation<T>
    {
        protected UnityWebRequest WebRequest { get; private set; }
        public T Result { get; private set; }
        
        protected NetworkOperation()
        {
            OnStart += SendRequest;
            ProgressGetter = GetWebRequestProgress;
            ResultCreation = CreateResult;
            SuccessCondition += IsNetworkOperationSuccess;
        }

        protected virtual float GetWebRequestProgress()
        {
            return WebRequest.downloadProgress;
        }

        protected override bool IsShouldFinish => WebRequest.isDone;

        void SendRequest()
        {
            WebRequest = CreateWebRequest();
            WebRequest.SendWebRequest();
        }

        protected abstract UnityWebRequest CreateWebRequest();

        T CreateResult()
        {
            SetResult(CreateResultFromWebRequest(WebRequest));
            return Result;
        }

        protected void SetResult(T value)
        {
            Result = value;
        }

        public override void ProcessResult(T result)
        {
            base.ProcessResult(result);
            WebRequest.Dispose();
        }
        
#if UNITASK
        public async UniTask<T> ResultTask()
        {
            await Task();
            if (Result == null)
                throw new OperationCanceledException();
            
            return Result;
        }
#endif

        protected virtual bool IsNetworkOperationSuccess()
        {
            return string.IsNullOrEmpty(WebRequest.error);
        }

        protected abstract T CreateResultFromWebRequest(UnityWebRequest request);
    }
}