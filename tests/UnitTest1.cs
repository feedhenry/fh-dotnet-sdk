using System.Threading.Tasks;
using FHSDK;
using FHSDK.Services;
using FHSDK.Services.Device;
using FHSDKPortable;
using Xunit;
using tests.Mocks;

namespace tests
{
    public class UnitTest1
    {
        [Fact]
        public async Task TestInit()
        {
            //given
            ServiceFinder.RegisterType<IDeviceService, TestDevice>();
            await FHClient.Init();

            //when
            var host = FH.GetCloudHost();

            //then
            Assert.True(host.Contains("HOST"));
        }
    }
}
