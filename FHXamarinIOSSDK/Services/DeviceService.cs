using System;
using System.Threading.Tasks;

namespace FHSDK.Services
{
	public class DeviceService : IDeviceService
	{
		public DeviceService ()
		{
		}

		public String GetDeviceId()
		{
			return null;
		}

		public string GetData(string dataId)
		{
			return null;
		}

		public void SaveData(string dataId, string dataValue)
		{
		}

		public Task<string> ReadResourceAsString(string resourceName)
		{
			return null;
		}

	}
}

