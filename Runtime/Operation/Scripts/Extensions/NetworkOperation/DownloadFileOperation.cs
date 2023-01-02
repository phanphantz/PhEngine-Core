using UnityEngine;
using UnityEngine.Networking;

namespace PhEngine.Core.Operation
{
    public class DownloadFileOperation : NetworkOperation<DownloadFileResult>
    {
        DownloadFileRequest Request { get; }

        public DownloadFileOperation(DownloadFileRequest request) : base(UnityWebRequest.Get(request.Url))
        {
            Request = request;
            OnSuccess += WriteDownloadedBytesToLocal;
            OnFail += ReportDownloadFailed;
        }

        protected override DownloadFileResult CreateResultFromWebRequest(UnityWebRequest request)
        {
            var fullPath = Request.GetPathToDownloadedFile();
            return new DownloadFileResult(fullPath , WebRequest.error);
        }
        
        void WriteDownloadedBytesToLocal(DownloadFileResult result)
        {
            var content = WebRequest.downloadHandler.data;
            FileIO.Write(result.ContentFullPath, content);
            Debug.Log($"Download Successful for URL : {Request.Url}\nFile Saved to : {Request.LocalSavePath}");
        }

        void ReportDownloadFailed(DownloadFileResult result)
        {
            Debug.LogError($"Download Failed for URL : {Request.Url}\nReason : {result.Error}");
        }
    }
}