using System;
using FHSDK;
using FHSDK.Services.Device;

namespace tests.Mocks
{
    public class TestDevice : IDeviceService
    {

        public string GetDeviceDestination()
        {
            return "DEVICE_DESTINATION";
        }

        public string GetDeviceId()
        {
            return "DEVICE_ID";
        }

        public AppProps ReadAppProps()
        {
            var props = new AppProps();
            props.host = "HOST";
            props.projectid = "PROJECT_ID";
            props.appid = "APP_ID";
            props.appkey = "APP_KEY";
            props.connectiontag = "CONNECTION_TAG";
            props.IsLocalDevelopment = true;
            return props;
        }

    }
}
