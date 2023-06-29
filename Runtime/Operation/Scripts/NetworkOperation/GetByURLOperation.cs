using PhEngine.Core.Operation;
using UnityEngine.Networking;

namespace PhEngine.Core.Operation
{
    public abstract class GetByURLOperation<T> : NetworkOperation<T>
    {
        public string Url { get; }
        protected GetByURLOperation(string url)
        {
            Url = url;
        }

        protected override UnityWebRequest CreateWebRequest()
        {
            return UnityWebRequest.Get(Url);
        }

        protected override T CreateResultFromWebRequest(UnityWebRequest request)
        {
            return IsNetworkOperationSuccess() ? CreateResultFromDownloadHandler(WebRequest.downloadHandler) : default;
        }

        protected abstract T CreateResultFromDownloadHandler(DownloadHandler handler);
    }
}