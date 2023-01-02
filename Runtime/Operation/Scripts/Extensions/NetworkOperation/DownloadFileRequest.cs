using System.IO;
using UnityEngine;

namespace PhEngine.Core.Operation
{
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
}