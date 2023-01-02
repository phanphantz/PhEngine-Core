using System;
using UnityEngine.Networking;

namespace PhEngine.Core.Operation
{
    [Serializable]
    public abstract class NetworkOperation<T> : RequestOperation<T>
    {
        protected UnityWebRequest WebRequest { get; private set; }

        protected NetworkOperation(UnityWebRequest webRequest)
        {
            WebRequest = webRequest;
            OnStart += SendRequest;
            ProgressGetter = GetWebRequestProgress;
            ResultCreation = CreateResult;
            SuccessCondition = IsWebRequestHasNoError;
        }

        protected virtual float GetWebRequestProgress()
        {
            return WebRequest.downloadProgress;
        }

        protected override bool IsShouldFinish()
        {
            return base.IsShouldFinish() && WebRequest.isDone;
        }

        void SendRequest()
        {
            WebRequest.SendWebRequest();
        }

        T CreateResult()
        {
            return CreateResultFromWebRequest(WebRequest);
        }
        
        protected virtual bool IsWebRequestHasNoError()
        {
            return string.IsNullOrEmpty(WebRequest.error);
        }

        protected abstract T CreateResultFromWebRequest(UnityWebRequest request);
    }
}