using System.IO.IsolatedStorage;
using FHSDK.Services.Data;

namespace FHSDK.Services.Data
{
    /// <summary>
    ///     On device data service provider for windows phone
    /// </summary>
    internal class DataService : DataServiceBase
    {
        protected override string DoRead(string dataId)
        {
            var settings = IsolatedStorageSettings.ApplicationSettings;
            string retvalue = null;
            settings.TryGetValue(dataId, out retvalue);
            return retvalue;
        }

        protected override void DoSave(string dataId, string dataValue)
        {
            var settings = IsolatedStorageSettings.ApplicationSettings;
            if (!settings.Contains(dataId))
            {
                settings.Add(dataId, dataValue);
            }

            settings.Save();
        }

        public override void DeleteData(string dataId)
        {
            var settings = IsolatedStorageSettings.ApplicationSettings;
            settings.Remove(dataId);
            settings.Save();
        }
    }
}