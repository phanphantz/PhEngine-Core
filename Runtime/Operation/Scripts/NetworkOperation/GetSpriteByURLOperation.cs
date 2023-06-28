using UnityEngine;
using UnityEngine.Networking;

namespace PhEngine.Core.Operation
{
    public class GetSpriteByURLOperation : NetworkOperation<Sprite>
    {
        string Url { get; }

        public GetSpriteByURLOperation(string url) : base(UnityWebRequestTexture.GetTexture(url))
        {
            Url = url;
            OnSuccess += ReportDownloadSuccess;
            OnFail += ReportDownloadFailed;
        }
        
        void ReportDownloadSuccess(Sprite texture)
        {
            Debug.Log("Get Sprite Successful! from URL : " + Url);
        }

        void ReportDownloadFailed(Sprite texture)
        {
            Debug.LogError("Get Sprite Failed because : " + WebRequest.error + "\nFrom URL : " + Url);
        }

        protected override Sprite CreateResultFromWebRequest(UnityWebRequest request)
        {
            var texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
            return SpriteCreator.CreateFromTexture(texture);
        }
    }
}