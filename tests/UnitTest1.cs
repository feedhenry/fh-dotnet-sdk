using System.Threading.Tasks;
using FHSDK;
using FHSDKPortable;
using Xunit;

namespace tests
{
    public class UnitTest1
    {
        [Fact]
        public async Task TestMethod1()
        {
            await FHClient.Init();
            var host = FH.GetCloudHost();
            Assert.True(host.Contains("http://192.168.28.34:8001"));

        }
    }
}
