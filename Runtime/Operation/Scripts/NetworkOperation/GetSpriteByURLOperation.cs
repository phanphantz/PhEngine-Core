using UnityEngine;
using UnityEngine.Networking;

namespace PhEngine.Core.Operation
{
    public class GetSpriteByURLOperation : NetworkOperation<Sprite>
    {
        public string Url { get; }
        
        public GetSpriteByURLOperation(string url)
        {
            Url = url;
            OnSuccess += ReportDownloadSuccess;
            OnFail += ReportDownloadFailed;
        }

        protected override UnityWebRequest CreateWebRequest()
        {
            return UnityWebRequestTexture.GetTexture(Url);
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
            if (request.error != null)
                return null;
            
            var texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
            return SpriteCreator.CreateFromTexture(texture);
        }
    }
}