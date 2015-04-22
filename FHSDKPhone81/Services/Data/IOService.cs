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
            StorageFile file = GetFile(fullPath);
            return FileIO.ReadTextAsync(file).AsTask().Result;
        }

        public void WriteFile(string fullPath, string content)
        {
            var file = GetFile(fullPath);
            FileIO.WriteTextAsync(file, content).AsTask().Wait();
        }

        private static StorageFile GetFile(string fullPath)
        {
            return StorageFile.GetFileFromPathAsync(fullPath).AsTask().Result;
        }

        public bool Exists(string fullPath)
        {
            try
            {
                GetFile(fullPath);
                return true;
            }
            catch (FileNotFoundException e)
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
