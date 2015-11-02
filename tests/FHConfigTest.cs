using System.Threading.Tasks;
using Xunit;
using FHSDK.Config;
using FHSDK.Services;
using FHSDK.Services.Device;
using tests.Mocks;

namespace tests
{
    public class FHConfigTest
    {
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
}
