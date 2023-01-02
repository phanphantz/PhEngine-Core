namespace PhEngine.Core.Operation
{
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