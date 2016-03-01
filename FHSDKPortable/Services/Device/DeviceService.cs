using System;
using System.IO;
using Windows.ApplicationModel;
using Windows.Security.Cryptography;
using Windows.Security.Cryptography.Core;
using Windows.Storage;
using Windows.System.Profile;
using Newtonsoft.Json;

namespace FHSDK.Services.Device
{
    /// <summary>
    ///     Device info service for windows phone
    /// </summary>
    internal class DeviceService : DeviceServiceBase
    {
        public override string GetDeviceId()
        {
            var token = HardwareIdentification.GetPackageSpecificToken(null);
            var hardwareId = token.Id;

            var hasher = HashAlgorithmProvider.OpenAlgorithm("MD5");
            var hashed = hasher.HashData(hardwareId);

            var hashedString = CryptographicBuffer.EncodeToHexString(hashed);
            return hashedString;
        }

        public override AppProps ReadAppProps()
        {
            AppProps appProps;
            var isLocalDev = false;
            var file = GetFile(Constants.LocalConfigFileName);
            if (null != file)
            {
                isLocalDev = true;
            }
            else
            {
                file = GetFile(Constants.ConfigFileName);
            }
            if (null != file)
            {
                var json = FileIO.ReadTextAsync(file).AsTask().Result;
                appProps = JsonConvert.DeserializeObject<AppProps>(json);
                appProps.IsLocalDevelopment = isLocalDev;
            }
            else
            {
                throw new IOException("Can not find resource " + Constants.ConfigFileName);
            }
            return appProps;
        }

        public override string GetDeviceDestination()
        {
            return "windows";
        }

        public override string GetPackageDir()
        {
            return Package.Current.InstalledLocation.Path;
        }

        private static StorageFile GetFile(string fileName)
        {
            StorageFile file = null;
            try
            {
                var folder = Package.Current.InstalledLocation;
                file = folder.GetFileAsync(fileName).AsTask().Result;
            }
            catch (AggregateException)
            {
            }
            return file;
        }
    }
}