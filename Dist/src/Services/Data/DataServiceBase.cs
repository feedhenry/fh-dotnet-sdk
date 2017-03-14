using System;
using FHSDK.Services.Log;

namespace FHSDK.Services.Data
{
    /// <summary>
    /// Abstract base class to provide CRUD access to data.
    /// </summary>
    public abstract class DataServiceBase : IDataService
    {
        private const string TAG = "FHSDK:DataService";

        /// <summary>
        /// Save data to local storage.
        /// </summary>
        /// <param name="dataId"></param>
        /// <param name="dataValue"></param>
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

        /// <summary>
        /// Read data from local storage.
        /// </summary>
        /// <param name="dataId"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Delete data from local storage.
        /// </summary>
        /// <param name="dataId"></param>
        public abstract void DeleteData(string dataId);

        /// <summary>
        /// Refer to GetData.
        /// </summary>
        /// <param name="dataId"></param>
        /// <returns></returns>
        protected abstract string DoRead(string dataId);

        /// <summary>
        /// Refer to SaveData.
        /// </summary>
        /// <param name="dataId"></param>
        /// <param name="dataValue"></param>
        protected abstract void DoSave(string dataId, string dataValue);
    }
}