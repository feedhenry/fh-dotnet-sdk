using System;
using System.Threading.Tasks;
using FHSDK.Services;

namespace FHSDK
{
	public class FHConfig
	{
		private AppProps appProps = null;
		private string destination = null;
		private string deviceid = null;
		private static FHConfig instance = null;

		private FHConfig (AppProps props, string dest, string uuid)
		{
			appProps = props;
			destination = dest;
			deviceid = uuid;
		}

		public static FHConfig getInstance()
		{
			if (null == instance) {
				IDeviceService deviceService = (IDeviceService) ServiceFinder.Resolve<IDeviceService>();
				AppProps props = deviceService.ReadAppProps ();
				string dest = deviceService.GetDeviceDestination ();
				string uuid = deviceService.GetDeviceId ();
				instance = new FHConfig (props, dest, uuid);

			}
			return instance;
		}

		public string GetHost()
		{
			return this.appProps.host;
		}

		public string GetAppId()
		{
			return this.appProps.appid;
		}

		public string GetAppKey()
		{
			return this.appProps.appkey;
		}

		public string GetProjectId()
		{
			return this.appProps.projectid;
		}

		public string GetMode(){
			return this.appProps.mode;
		}

		public string GetConnectionTag(){
			return this.appProps.connectiontag;
		}

		public string GetDestination(){
			return this.destination;
		}

		public string GetDeviceId(){
			return this.deviceid;
		}


	}
}

