using System;
using UnityEngine.Networking;

namespace PhEngine.Core.Operation
{
    [Serializable]
    public abstract class NetworkOperation<T> : RequestOperation<T>
    {
        protected UnityWebRequest WebRequest { get; private set; }

        bool isExpired;

        protected NetworkOperation(UnityWebRequest webRequest)
        {
            AssignWebRequest(webRequest);
            OnStart += SendRequest;
            ProgressGetter = GetWebRequestProgress;
            ResultCreation = CreateResult;
            SuccessCondition = IsNetworkOperationSuccess;
        }

        protected void AssignWebRequest(UnityWebRequest webRequest)
        {
            WebRequest = webRequest;
        }

        protected virtual float GetWebRequestProgress()
        {
            if (isExpired)
                return 1f;
            
            return WebRequest.downloadProgress;
        }

        protected override bool IsShouldFinish()
        {
            if (isExpired)
                return true;
            
            return WebRequest.isDone;
        }

        void SendRequest()
        {
            WebRequest.SendWebRequest();
        }

        T CreateResult()
        {
            return CreateResultFromWebRequest(WebRequest);
        }

        public override void ProcessResult(T result)
        {
            base.ProcessResult(result);
            WebRequest.Dispose();
            isExpired = true;
        }

        protected virtual bool IsNetworkOperationSuccess()
        {
            return string.IsNullOrEmpty(WebRequest.error);
        }

        protected abstract T CreateResultFromWebRequest(UnityWebRequest request);
    }
}