using System.IO;
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
    
    public class DownloadFileRequest
    {
        public string Url { get; }
        public string LocalSavePath { get; }

        public DownloadFileRequest
        (
            string url,
            string localSavePath
        )
        {
            Url = url;
            LocalSavePath = localSavePath;
        }
        
        public string GetPathToDownloadedFile()
        {
            var fullContentPath = Path.Combine
            (
                Application.persistentDataPath,
                LocalSavePath
            );
            return fullContentPath;
        }
    }
    
    public class DownloadFileResult
    {
        public string ContentFullPath { get; }
        public string Error { get; }
        public bool IsError => !string.IsNullOrEmpty(Error);
        
        public DownloadFileResult(string contentFullPath, string error)
        {
            ContentFullPath = contentFullPath;
            Error = error;
        }
    }
}