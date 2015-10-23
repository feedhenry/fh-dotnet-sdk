using System.Threading.Tasks;
using FHSDK;
using FHSDKPortable;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;

namespace tests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public async Task TestMethod1()
        {
            await FHClient.Init();
            var host = FH.GetCloudHost();
            Assert.IsTrue(host.Contains("http://192.168.28.34:8001"));

        }
    }
}
