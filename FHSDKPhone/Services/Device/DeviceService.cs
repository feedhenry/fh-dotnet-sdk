using System;
using System.IO;
using System.Windows;
using Microsoft.Phone.Info;
using Newtonsoft.Json;

namespace FHSDK.Services.Device
{
    /// <summary>
    ///     Device info service for windows phone
    /// </summary>
    internal class DeviceService : IDeviceService
    {
        public string GetDeviceId()
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

        public AppProps ReadAppProps()
        {
            AppProps appProps = null;
            var isLocalDev = false;
            StreamResourceInfo streamInfo = Application.GetResourceStream(new Uri(Constants.LOCAL_CONFIG_FILE_NAME, UriKind.Relative));
            if (null != streamInfo)
            {
                isLocalDev = true;
            }
            else
            {
                streamInfo = Application.GetResourceStream(new Uri(Constants.CONFIG_FILE_NAME, UriKind.Relative));
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
                throw new IOException("Can not find resource " + Constants.CONFIG_FILE_NAME);
            }
            return appProps;
        }

        public string GetDeviceDestination()
        {
            return "windowsphone8";
        }
    }
}