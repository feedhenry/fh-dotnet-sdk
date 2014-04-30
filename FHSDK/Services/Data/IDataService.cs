using System;

namespace FHSDK.Services
{
	public interface IDataService
	{
		void SaveData(string dataId, string dataValue);
		string GetData(string dataId);
	}
}

