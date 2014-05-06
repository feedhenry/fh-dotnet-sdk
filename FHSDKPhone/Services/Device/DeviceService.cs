using Microsoft.Phone.Info;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Resources;

namespace FHSDK.Services
{
    class DeviceService : IDeviceService
    {

        private const string CONFIG_FILE_NAME = "fhconfig.json";

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
                     retVal = Convert.ToBase64String((byte[])uuid);
                 }
            }
            return retVal;
        }

        public AppProps ReadAppProps()
        {
             AppProps appProps = null;
            StreamResourceInfo streamInfo = Application.GetResourceStream(new Uri(CONFIG_FILE_NAME, UriKind.Relative));
            if (null != streamInfo)
            {
                StreamReader sr = new StreamReader(streamInfo.Stream);
                string fileContent = sr.ReadToEnd();
                if(null != fileContent)
                {
                    appProps = JsonConvert.DeserializeObject<AppProps>(fileContent);
                }
            }
            else
            {
                throw new IOException("Can not find resource " + CONFIG_FILE_NAME);
            }
            return appProps;
        }

        public string GetDeviceDestination()
        {
            return "windowsphone";
        }
    }
}
