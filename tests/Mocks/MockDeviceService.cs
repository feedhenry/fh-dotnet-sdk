using Windows.ApplicationModel;
using FHSDK;
using FHSDK.Services.Device;

namespace tests.Mocks
{
    public class MockDeviceService : IDeviceService
    {
        internal const string Host = "HOST";
        internal const string ProjectId = "PROJECT_ID";
        internal const string AppId = "APP_ID";
        internal const string AppKey = "APP_KEY";
        internal const string ConnectionTag = "CONNECTION_TAG";
        internal const string DeviceDestination = "DEVICE_DESTINATION";
        internal const string DeviceId = "DEVICE_ID";

        public string GetDeviceDestination()
        {
            return DeviceDestination;
        }

        public string GetPackageDir()
        {
            return Package.Current.InstalledLocation.Path;
        }

        public string GetDeviceId()
        {
            return DeviceId;
        }

        public AppProps ReadAppProps()
        {
            var props = new AppProps
            {
                host = Host,
                projectid = ProjectId,
                appid = AppId,
                appkey = AppKey,
                connectiontag = ConnectionTag,
                IsLocalDevelopment = true
            };
            return props;
        }
    }
}