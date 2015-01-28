using System;
using System.IO;
using Windows.Storage;
using FHSDK81.Phone;

namespace FHSDK.Services
{
    class IOService : IIOService
    {
        public IOService()
        {

        }

        public string ReadFile(string fullPath)
        {
            StorageFile file;
            try
            {
                file = GetFile(fullPath);
                return AsyncHelpers.RunSync<string>(() => FileIO.ReadTextAsync(file).AsTask());
            }
            catch (AggregateException e)
            {
                return null;
            }
        }

        public void WriteFile(string fullPath, string content)
        {
            var file = GetFile(fullPath);
            AsyncHelpers.RunSync(() => FileIO.WriteTextAsync(file, content).AsTask());
        }

        private static StorageFile GetFile(string fullPath)
        {
            return AsyncHelpers.RunSync<StorageFile>(() => StorageFile.GetFileFromPathAsync(fullPath).AsTask());
        }

        public bool Exists(string fullPath)
        {
            try
            {
                GetFile(fullPath);
                return true;
            }
            catch (AggregateException e)
            {
                return false;
            }
        }

        public string GetDataPersistDir()
        {
            StorageFolder local = Windows.Storage.ApplicationData.Current.LocalFolder;
            return local.Path;
        }
    }


   

}
