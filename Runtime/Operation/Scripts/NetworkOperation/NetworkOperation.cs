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
            if (webRequest == null)
                throw new ArgumentNullException(nameof(webRequest));
                
            WebRequest = webRequest;
            OnStart += SendRequest;
            ProgressGetter = GetWebRequestProgress;
            ResultCreation = CreateResult;
            SuccessCondition = IsWebRequestHasNoError;
        }

        protected virtual float GetWebRequestProgress()
        {
            if (WebRequest == null)
                return 1f;
            
            return WebRequest.downloadProgress;
        }

        protected override bool IsShouldFinish()
        {
            if (WebRequest == null)
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
        }

        protected virtual bool IsWebRequestHasNoError()
        {
            return string.IsNullOrEmpty(WebRequest.error);
        }

        protected abstract T CreateResultFromWebRequest(UnityWebRequest request);
    }
}