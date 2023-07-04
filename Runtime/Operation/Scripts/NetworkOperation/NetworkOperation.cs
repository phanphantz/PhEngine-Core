using System;
using UnityEngine.Networking;

namespace PhEngine.Core.Operation
{
    [Serializable]
    public abstract class NetworkOperation<T> : RequestOperation<T>
    {
        protected UnityWebRequest WebRequest { get; private set; }
        
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

        protected override bool IsShouldFinish()
        {
            return WebRequest.isDone;
        }

        void SendRequest()
        {
            WebRequest = CreateWebRequest();
            WebRequest.SendWebRequest();
        }

        protected abstract UnityWebRequest CreateWebRequest();

        T CreateResult()
        {
            return CreateResultFromWebRequest(WebRequest);
        }

        public override void ProcessResult(T result)
        {
            base.ProcessResult(result);
            WebRequest.Dispose();
        }

        protected virtual bool IsNetworkOperationSuccess()
        {
            return string.IsNullOrEmpty(WebRequest.error);
        }

        protected abstract T CreateResultFromWebRequest(UnityWebRequest request);
    }
}