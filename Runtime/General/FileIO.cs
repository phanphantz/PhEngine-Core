using System.IO;
using UnityEngine;

namespace PhEngine.Core
{
    public static class FileIO
    {
        public static void Write(string localPath, byte[] content)
        {
            var fullPath = Path.Combine(Application.persistentDataPath, localPath);
            var directoryPath = Path.GetDirectoryName(fullPath);
            CreateDirectoryIfNotExist(directoryPath);
            
            File.WriteAllBytes(fullPath, content);
        }

        public static void CreateDirectoryIfNotExist(string directoryPath)
        {
            if (!Directory.Exists(directoryPath))
                Directory.CreateDirectory(directoryPath);
        }
    }
}