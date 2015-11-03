using System.Threading.Tasks;
using FHSDK.Config;
using FHSDKPortable;
using tests.Mocks;
using Xunit;

namespace tests.Config
{
    public class FHConfigTest
    {
        [Fact]
        public void TestReadConfigWithMockedDevice()
        {
            // given a mocked DeviceService
            var config = new FHConfig(new MockDeviceService());

            // when
            // default instanciation

            // then
            Assert.Equal(MockDeviceService.Host, config.GetHost());
            Assert.Equal(MockDeviceService.ProjectId, config.GetProjectId());
            Assert.Equal(MockDeviceService.AppKey, config.GetAppKey());
            Assert.Equal(MockDeviceService.AppId, config.GetAppId());
            Assert.Equal(MockDeviceService.ConnectionTag, config.GetConnectionTag());
            Assert.Equal(MockDeviceService.DeviceDestination, config.GetDestination());
            Assert.Equal(MockDeviceService.DeviceId, config.GetDeviceId());
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