using Microsoft.Phone.Info;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Resources;

namespace FHSDK.Services
{
    class DeviceService : IDeviceService
    {
        public DeviceService()
        {
        }

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

        public void SaveData(string dataId, string dataValue)
        {
            IsolatedStorageSettings settings = IsolatedStorageSettings.ApplicationSettings;
            settings.Add(dataId, dataValue);
            settings.Save();
        }

        public string GetData(string dataId)
        {
            IsolatedStorageSettings settings = IsolatedStorageSettings.ApplicationSettings;
            string retvalue = null;
            settings.TryGetValue(dataId, out retvalue);
            return retvalue;
        }

        public async Task<string> ReadResourceAsString(string resourceName)
        {
            StreamResourceInfo streamInfo = Application.GetResourceStream(new Uri(resourceName, UriKind.Relative));
            if (null != streamInfo)
            {
                StreamReader sr = new StreamReader(streamInfo.Stream);
                return await sr.ReadToEndAsync();
            }
            else
            {
                throw new IOException("Can not find resource " + resourceName);
            }
        }
    }

   
}
