using System;
using System.IO;
using System.Windows;
using Windows.ApplicationModel;
using Microsoft.Phone.Info;
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
            string retVal = null;
            object uuid;
            UserExtendedProperties.TryGetValue("ANID2", out uuid);
            if (null != uuid)
            {
                retVal = uuid.ToString().Substring(2, 32);
            }
            else
            {
                DeviceExtendedProperties.TryGetValue("DeviceUniqueId", out uuid);
                if (null != uuid)
                {
                    retVal = Convert.ToBase64String((byte[]) uuid);
                }
            }
            return retVal;
        }

        public override AppProps ReadAppProps()
        {
            AppProps appProps;
            var isLocalDev = false;
            var streamInfo = Application.GetResourceStream(new Uri(Constants.LocalConfigFileName, UriKind.Relative));
            if (null != streamInfo)
            {
                isLocalDev = true;
            }
            else
            {
                streamInfo = Application.GetResourceStream(new Uri(Constants.ConfigFileName, UriKind.Relative));
            }
            if (null != streamInfo)
            {
                var sr = new StreamReader(streamInfo.Stream);
                var fileContent = sr.ReadToEnd();
                appProps = JsonConvert.DeserializeObject<AppProps>(fileContent);
                if (isLocalDev)
                {
                    appProps.IsLocalDevelopment = true;
                }
            }
            else
            {
                throw new IOException("Can not find resource " + Constants.ConfigFileName);
            }
            return appProps;
        }

        public override string GetDeviceDestination()
        {
            return "windowsphone8";
        }

        public override string GetPackageDir()
        {
            return Package.Current.InstalledLocation.Path;
        }
    }
}