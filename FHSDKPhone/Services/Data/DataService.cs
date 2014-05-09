using System;
using System.Collections.Generic;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FHSDK.Services
{
    /// <summary>
    /// On device data service provider for windows phone
    /// </summary>
    class DataService: DataServiceBase
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public DataService() :
            base()
        {
        }

        protected override string doRead(string dataId)
        {
            IsolatedStorageSettings settings = IsolatedStorageSettings.ApplicationSettings;
            string retvalue = null;
            settings.TryGetValue(dataId, out retvalue);
            return retvalue;
        }

        protected override void doSave(string dataId, string dataValue)
        {
            IsolatedStorageSettings settings = IsolatedStorageSettings.ApplicationSettings;
            if (!settings.Contains(dataId))
            {
                settings.Add(dataId, dataValue);
            }

            settings.Save();
        }
    }
}
