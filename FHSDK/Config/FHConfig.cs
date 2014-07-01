using System;
using System.Threading.Tasks;
using FHSDK.Services;

namespace FHSDK
{
    /// <summary>
    /// Singleton class to return various configurations of the app.
    /// </summary>
	public class FHConfig
	{
		private AppProps appProps = null;
		private string destination = null;
		private string deviceid = null;
		private static FHConfig instance = null;

        public bool IsLocalDevelopment { get; private set; }

		private FHConfig (AppProps props, string dest, string uuid)
		{
			appProps = props;
			destination = dest;
			deviceid = uuid;
            if(props.IsLocalDevelopment){
                this.IsLocalDevelopment = true;
            } else {
                this.IsLocalDevelopment = false;
            }
		}

        /// <summary>
        /// Return the singleton instance of the class
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// Get the app's hosting server
        /// </summary>
        /// <returns>the host of the app</returns>
		public string GetHost()
		{
			return this.appProps.host;
		}

        /// <summary>
        /// Get the app id
        /// </summary>
        /// <returns>app id</returns>
		public string GetAppId()
		{
			return this.appProps.appid;
		}

        /// <summary>
        /// Get the app key
        /// </summary>
        /// <returns>app key</returns>
		public string GetAppKey()
		{
			return this.appProps.appkey;
		}

        /// <summary>
        /// Get the project id
        /// </summary>
        /// <returns>project id</returns>
		public string GetProjectId()
		{
			return this.appProps.projectid;
		}

        /// <summary>
        /// Get the mode of the app. Deprecated.
        /// </summary>
        /// <returns>app mode</returns>
		public string GetMode(){
			return this.appProps.mode;
		}

        /// <summary>
        /// Get the connection tag of the app
        /// </summary>
        /// <returns>the connection tag</returns>
		public string GetConnectionTag(){
			return this.appProps.connectiontag;
		}

        /// <summary>
        /// Get the device type the app is running on.
        /// </summary>
        /// <returns>device type. E.g ios, android , windowsphone</returns>
		public string GetDestination(){
			return this.destination;
		}

        /// <summary>
        /// Get the unique device id.
        /// </summary>
        /// <returns>the unique device id</returns>
		public string GetDeviceId(){
			return this.deviceid;
		}


	}
}

