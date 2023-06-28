using UnityEngine;
using UnityEngine.Networking;

namespace PhEngine.Core.Operation
{
    public class GetTextureByURLOperation : NetworkOperation<Texture>
    {
        string Url { get; }

        public GetTextureByURLOperation(string url) : base(UnityWebRequestTexture.GetTexture(url))
        {
            Url = url;
            OnSuccess += ReportDownloadSuccess;
            OnFail += ReportDownloadFailed;
        }
        
        void ReportDownloadSuccess(Texture texture)
        {
            Debug.Log("Get Texture Successful! from URL : " + Url);
        }

        void ReportDownloadFailed(Texture texture)
        {
            Debug.LogError("Get Texture Failed because : " + WebRequest.error + "\nFrom URL : " + Url);
        }

        protected override Texture CreateResultFromWebRequest(UnityWebRequest request)
        {
            return ((DownloadHandlerTexture) request.downloadHandler).texture;
        }
    }
}