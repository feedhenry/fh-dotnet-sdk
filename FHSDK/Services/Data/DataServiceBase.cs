using System;
using FHSDK.Services.Log;

namespace FHSDK.Services.Data
{
    public abstract class DataServiceBase : IDataService
    {
        private const string TAG = "FHSDK:DataService";

        public void SaveData(string dataId, string dataValue)
        {
            try
            {
                DoSave(dataId, dataValue);
            }
            catch (Exception ex)
            {
                var logger = ServiceFinder.Resolve<ILogService>();
                if (null != logger)
                {
                    logger.e(TAG, "Failed to save data", ex);
                }
            }
        }

        public string GetData(string dataId)
        {
            string data = null;
            try
            {
                data = DoRead(dataId);
            }
            catch (Exception ex)
            {
                var logger = ServiceFinder.Resolve<ILogService>();
                if (null != logger)
                {
                    logger.e(TAG, "Failed to read data", ex);
                }
            }
            return data;
        }

        public abstract void DeleteData(string dataId);
        protected abstract string DoRead(string dataId);
        protected abstract void DoSave(string dataId, string dataValue);
    }
}