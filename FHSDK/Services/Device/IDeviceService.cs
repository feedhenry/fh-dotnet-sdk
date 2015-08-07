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
        ///     Get the device type. E.g. iphone, android, windowsphone8
        /// </summary>
        /// <returns>the device type</returns>
        string GetDeviceDestination();
    }

    public static class Constants
    {
        public const string LocalConfigFileName = "fhconfig.local.json";
        public const string ConfigFileName = "fhconfig.json";
    }
}