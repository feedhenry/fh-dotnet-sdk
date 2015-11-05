using System.Threading.Tasks;
using Xunit;
using FHSDK.Config;
using FHSDK.Services;
using FHSDK.Services.Device;
using tests.Mocks;
using FHSDKPortable;

namespace tests
{
    public class FHConfigTest
    {
        [Fact]
        public async Task TestReadConfigWithMockedDevice()
        {
            // given a mocked DeviceService
            var config = new FHConfig(new TestDevice());

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

        
        [Fact]
        public async Task TestReadConfig()
        {
            // given a mocked DeviceService
            await FHClient.Init();
            var config = FHConfig.GetInstance();

            // when
            // default instanciation

            // then
            Assert.Equal("http://192.168.28.34:8001", config.GetHost());
            Assert.Equal("project_id_for_test", config.GetProjectId());
            Assert.Equal("app_key_for_test", config.GetAppKey());
            Assert.Equal("appid_for_test", config.GetAppId());
            Assert.Equal("connection_tag_for_test", config.GetConnectionTag());
        }
    }
}
