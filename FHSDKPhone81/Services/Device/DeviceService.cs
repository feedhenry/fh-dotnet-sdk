using FHSDK.Phone;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
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
            StorageFile file = AsyncHelpers.RunSync<StorageFile>(() => StorageFile.GetFileFromApplicationUriAsync(new Uri(LOCAL_CONFIG_FILE_NAME, UriKind.Relative)).AsTask());
            if (null != file)
            {
                IsLocalDev = true;
            }
            else
            {
                file = AsyncHelpers.RunSync<StorageFile>(() => StorageFile.GetFileFromApplicationUriAsync(new Uri(CONFIG_FILE_NAME, UriKind.Relative)).AsTask());
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

        public string GetDeviceDestination()
        {
            return "windowsphone8";
        }
    }
}
