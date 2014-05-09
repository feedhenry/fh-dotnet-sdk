using System;

namespace FHSDK.Services
{
	public abstract class DataServiceBase : IDataService
	{
		protected const String TAG = "FHSDK:DataService";
		public DataServiceBase ()
		{
		}

		public void SaveData(string dataId, string dataValue)
		{
			try {
				doSave(dataId, dataValue);
			} catch (Exception ex) {
				ILogService logger = ServiceFinder.Resolve<ILogService> ();
				if (null != logger) {
					logger.e (TAG, "Failed to save data", ex);
				}
			}
		}

		public string GetData(string dataId)
		{
			string data = null;
			try {
				data = doRead(dataId);
			} catch (Exception ex) {
				ILogService logger = ServiceFinder.Resolve<ILogService> ();
				if (null != logger) {
					logger.e (TAG, "Failed to read data", ex);
				}
			}
			return data;
		}

		protected abstract string doRead(string dataId);

		protected abstract void doSave(string dataId, string dataValue);

	}
}

