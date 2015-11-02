using System.Threading.Tasks;
using Xunit;
using FHSDK.Config;
using FHSDK.Services;
using FHSDK.Services.Device;
using FHSDK;
using System;

namespace tests
{
    public class FHConfigTest
    {
       // [Fact]
        public async Task TestReadConfig()
        {
            // given
            var config = FHConfig.GetInstance();

            // when
            // default instanciation

            // then
            Assert.Equal(@"http://192.168.28.34:8001", config.GetHost());
            Assert.Equal("project_id_for_test", config.GetProjectId());
            Assert.Equal("app_key_for_test", config.GetAppKey());
            Assert.Equal("appid_for_test", config.GetAppId());
            Assert.Equal("connection_tag_for_test", config.GetConnectionTag());
        }

        [Fact]
        public async Task TestReadConfigWithMockedDevice()
        {
            // given a mocked DeviceService
            ServiceFinder.RegisterType<IDeviceService, TestDevice>();
            var config = FHConfig.GetInstance();
            

            // when
            // default instanciation

            // then
            Assert.Equal("HOST", config.GetHost());
            Assert.Equal("PROJECT_ID", config.GetProjectId());
            Assert.Equal("APP_KEY", config.GetAppKey());
            Assert.Equal("APP_ID", config.GetAppId());
            Assert.Equal("CONNECTION_TAG", config.GetConnectionTag());
            Assert.Equal("DEVICE_DESTINATION", config.GetDestination());
            Assert.Equal("DEVICE_ID", config.GetDeviceId());
        }
    }

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
