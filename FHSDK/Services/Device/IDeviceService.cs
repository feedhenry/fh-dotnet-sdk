using AeroGear.Push;

namespace FHSDK.Services.Device
{
    /// <summary>
    ///     A service interface provides information about the device
    /// </summary>
    public interface IDeviceService
    {
        /// <summary>
        ///     Return the unique id of the device
        /// </summary>
        /// <returns>the unique device id</returns>
        string GetDeviceId();

        /// <summary>
        ///     Return the FeedHenry app configurations
        /// </summary>
        /// <returns>the FeedHenry app configurations </returns>
        AppProps ReadAppProps();

		/// <summary>
		///     Return the Push Notification configurations
		/// </summary>
		/// <returns>The push configuration for this app </returns>
		PushConfig ReadPushConfig ();

        /// <summary>
        ///     Get the device type. E.g. iphone, android, windowsphone8, windows
        /// </summary>
        /// <returns>the device type</returns>
        string GetDeviceDestination();

        /// <summary>
        ///     Get the installation directory of the app
        /// </summary>
        /// <returns>the directory the app was installed in</returns>
        string GetPackageDir();
    }
    /// <summary>
    /// Constant that holds default name for config files.
    /// </summary>
    public static class Constants
    {
        /// <summary>
        /// Development mode config file which override deafult config file.
        /// </summary>
        public const string LocalConfigFileName = "fhconfig.local.json";

        /// <summary>
        /// Default config file.
        /// </summary>
        public const string ConfigFileName = "fhconfig.json";
    }
}