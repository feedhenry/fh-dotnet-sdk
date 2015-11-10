#if __MOBILE__
using NUnit.Framework;
using TestClass = NUnit.Framework.TestFixtureAttribute;
using TestMethod = NUnit.Framework.TestAttribute;
#else
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
#endif

using System.Threading.Tasks;
using FHSDK;
using FHSDK.Config;
using tests.Mocks;

namespace tests
{
    [TestClass]
    public class ConfigTest
    {
        [TestMethod]
        public void TestReadConfigWithMockedDevice()
        {
            // given a mocked DeviceService
            var config = new FHConfig(new MockDeviceService());

            // when
            // default instanciation

            // then
            Assert.AreEqual(MockDeviceService.Host, config.GetHost());
            Assert.AreEqual(MockDeviceService.ProjectId, config.GetProjectId());
            Assert.AreEqual(MockDeviceService.AppKey, config.GetAppKey());
            Assert.AreEqual(MockDeviceService.AppId, config.GetAppId());
            Assert.AreEqual(MockDeviceService.ConnectionTag, config.GetConnectionTag());
            Assert.AreEqual(MockDeviceService.DeviceDestination, config.GetDestination());
            Assert.AreEqual(MockDeviceService.DeviceId, config.GetDeviceId());
        }


        [TestMethod]
        public async Task TestReadConfig()
        {
            // given a mocked DeviceService
            await FHClient.Init();
            var config = FHConfig.GetInstance();

            // when
            // default instanciation

            // then
            Assert.AreEqual("http://192.168.28.34:8001", config.GetHost());
            Assert.AreEqual("project_id_for_test", config.GetProjectId());
            Assert.AreEqual("app_key_for_test", config.GetAppKey());
            Assert.AreEqual("appid_for_test", config.GetAppId());
            Assert.AreEqual("connection_tag_for_test", config.GetConnectionTag());
        }
    }
}