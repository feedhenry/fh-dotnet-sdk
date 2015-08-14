using System;
using System.IO;
using Windows.Storage;

namespace FHSDK.Services.Data
{
    internal class IOService : IIOService
    {
        public string ReadFile(string fullPath)
        {
            var file = GetFile(fullPath);
            return FileIO.ReadTextAsync(file).AsTask().Result;
        }

        public void WriteFile(string fullPath, string content)
        {
            var file = GetFile(fullPath);
            FileIO.WriteTextAsync(file, content).AsTask().Wait();
        }

        public bool Exists(string fullPath)
        {
            try
            {
                GetFile(fullPath);
                return true;
            }
            catch (FileNotFoundException)
            {
                return false;
            }
        }

        public string GetDataPersistDir()
        {
            var local = ApplicationData.Current.LocalFolder;
            return local.Path;
        }

        private static StorageFile GetFile(string fullPath)
        {
            return StorageFile.GetFileFromPathAsync(fullPath).AsTask().Result;
        }
    }
}