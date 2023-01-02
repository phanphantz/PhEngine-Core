using PhEngine.Core.Operation;
using UnityEngine.Networking;

namespace PhEngine.Core.Operation
{
    public abstract class GetByURLOperation<T> : NetworkOperation<T>
    {
        protected GetByURLOperation(string url) : base(UnityWebRequest.Get(url))
        {
        }

        protected override T CreateResultFromWebRequest(UnityWebRequest request)
        {
            return IsWebRequestHasNoError() ? CreateResultFromDownloadHandler(WebRequest.downloadHandler) : default;
        }

        protected abstract T CreateResultFromDownloadHandler(DownloadHandler handler);
    }
}