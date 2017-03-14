using FHSDK.Services;
using FHSDK.Services.Device;

namespace FHSDK.Config
{
    /// <summary>
    /// Singleton class to return various configurations of the app.
    /// </summary>
    public class FHConfig
    {
        private static FHConfig _instance;
        private readonly AppProps _appProps;
        private readonly string _destination;
        private readonly string _deviceid;

        private FHConfig(AppProps props, string dest, string uuid)
        {
            _appProps = props;
            _destination = dest;
            _deviceid = uuid;
            IsLocalDevelopment = props.IsLocalDevelopment;
        }
        /// <summary>
        /// Initializer used for unit testing.
        /// </summary>
        /// <param name="deviceService"></param>
        public FHConfig(IDeviceService deviceService)
        {
            _appProps = deviceService.ReadAppProps();
            _destination = deviceService.GetDeviceDestination();
            _deviceid = deviceService.GetDeviceId();
        }

        /// <summary>
        /// Whether or not this is a "development" mode config.
        /// </summary>
        public bool IsLocalDevelopment { get; private set; }

        /// <summary>
        /// Return the singleton instance of the class.
        /// </summary>
        /// <returns></returns>
        public static FHConfig GetInstance()
        {
            if (null != _instance) return _instance;
            var deviceService = ServiceFinder.Resolve<IDeviceService>();
            var props = deviceService.ReadAppProps();
            var dest = deviceService.GetDeviceDestination();
            var uuid = deviceService.GetDeviceId();
            _instance = new FHConfig(props, dest, uuid);
            return _instance;
        }

        /// <summary>
        /// Get the app's hosting server.
        /// </summary>
        /// <returns>the host of the app</returns>
        public string GetHost()
        {
            return _appProps.host;
        }

        /// <summary>
        /// Get the app id.
        /// </summary>
        /// <returns>app id</returns>
        public string GetAppId()
        {
            return _appProps.appid;
        }

        /// <summary>
        /// Get the app key.
        /// </summary>
        /// <returns>app key</returns>
        public string GetAppKey()
        {
            return _appProps.appkey;
        }

        /// <summary>
        /// Get the project id.
        /// </summary>
        /// <returns>project id</returns>
        public string GetProjectId()
        {
            return _appProps.projectid;
        }

        /// <summary>
        /// Get the mode of the app. Deprecated.
        /// </summary>
        /// <returns>app mode</returns>
        public string GetMode()
        {
            return _appProps.mode;
        }

        /// <summary>
        /// Get the connection tag of the app.
        /// </summary>
        /// <returns>the connection tag</returns>
        public string GetConnectionTag()
        {
            return _appProps.connectiontag;
        }

        /// <summary>
        /// Get the device type the app is running on.
        /// </summary>
        /// <returns>device type. E.g ios, android , windowsphone</returns>
        public string GetDestination()
        {
            return _destination;
        }

        /// <summary>
        /// Get the unique device id.
        /// </summary>
        /// <returns>the unique device id</returns>
        public string GetDeviceId()
        {
            return _deviceid;
        }
    }
}