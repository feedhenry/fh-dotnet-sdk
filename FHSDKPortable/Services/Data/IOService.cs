using System;
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
            var file = CreateIfNotExists(fullPath);
            FileIO.WriteTextAsync(file, content).AsTask().Wait();
        }

        public bool Exists(string fullPath)
        {
            try
            {
                GetFile(fullPath);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public string GetDataPersistDir()
        {
            return ApplicationData.Current.LocalFolder.Path;
        }

        private static StorageFile GetFile(string fullPath)
        {
            return StorageFile.GetFileFromPathAsync(fullPath).AsTask().Result;
        }

        private StorageFile CreateIfNotExists(string fullPath)
        {
            if (!Exists(fullPath))
            {
                var path = fullPath.Substring(GetDataPersistDir().Length);
                return ApplicationData.Current.LocalFolder.CreateFileAsync(path, CreationCollisionOption.OpenIfExists).AsTask().Result;
            }

            return StorageFile.GetFileFromPathAsync(fullPath).AsTask().Result;
        }
    }
}