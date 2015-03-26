using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

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
            var settings = ApplicationData.Current.LocalSettings;
            return (string) settings.Values[dataId];
        }

        protected override void doSave(string dataId, string dataValue)
        {
            var settings = ApplicationData.Current.LocalSettings;
            settings.Values[dataId] = dataValue;
        }
    }
}
