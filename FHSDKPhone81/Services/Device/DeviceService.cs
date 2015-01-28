using FHSDK;
using FHSDK81.Phone;
using Newtonsoft.Json;
using System;
using System.IO;
using Windows.ApplicationModel;
using Windows.Security.Cryptography;
using Windows.Security.Cryptography.Core;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.System.Profile;

namespace FHSDK.Services
{
    /// <summary>
    /// Device info service for windows phone
    /// </summary>
    class DeviceService : IDeviceService
    {

        private const string LOCAL_CONFIG_FILE_NAME = "fhconfig.local.json";
        private const string CONFIG_FILE_NAME = "fhconfig.json";

        public string GetDeviceId()
        {
            HardwareToken token = HardwareIdentification.GetPackageSpecificToken(null);
            IBuffer hardwareId = token.Id;

            HashAlgorithmProvider hasher = HashAlgorithmProvider.OpenAlgorithm("MD5");
            IBuffer hashed = hasher.HashData(hardwareId);

            string hashedString = CryptographicBuffer.EncodeToHexString(hashed);
            return hashedString;
        }

        public AppProps ReadAppProps()
        {
            AppProps appProps = null;
            bool IsLocalDev = false;
            StorageFile file = GetFile(LOCAL_CONFIG_FILE_NAME);
            if (null != file)
            {
                IsLocalDev = true;
            }
            else
            {
                file = GetFile(CONFIG_FILE_NAME);                
            }
            if (null != file)
            {
                var json = AsyncHelpers.RunSync<string>(() => FileIO.ReadTextAsync(file).AsTask());
                appProps = JsonConvert.DeserializeObject<AppProps>(json);
                appProps.IsLocalDevelopment = IsLocalDev;
            }
            else
            {
                throw new IOException("Can not find resource " + CONFIG_FILE_NAME);
            }
            return appProps;
        }

        private static StorageFile GetFile(string fileName)
        {
            StorageFile file = null;
            try
            {
                var folder = Package.Current.InstalledLocation;
                file = AsyncHelpers.RunSync<StorageFile>(() => folder.GetFileAsync(fileName).AsTask<StorageFile>());
            }
            catch (AggregateException e) { }
            return file;
        }

        public string GetDeviceDestination()
        {
            return "windowsphone8";
        }
    }
}
