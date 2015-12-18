
using System.Collections.Generic;
#if __MOBILE__
using NUnit.Framework;
using TestClass = NUnit.Framework.TestFixtureAttribute;
using TestMethod = NUnit.Framework.TestAttribute;
#else
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
#endif

using System.Threading.Tasks;
using AeroGear.Push;
using FHSDK;
using FHSDK.Services.Network;

namespace tests
{
    [TestClass]
    public class PushBaseTest
    {
        [TestMethod]
        public async Task TestLoadingPushConfig()
        {
            //given
            await FHClient.Init();

            //when
            var config = MockPush.GetConfig();

            //then
            Assert.AreEqual("http://192.168.28.34:8001/api/v2/ag-push", config.UnifiedPushUri);
            Assert.AreEqual(new List<string>() {"one", "two"}, config.Categories);
        }
    }

    public class MockPush : PushBase
    {
        protected override Registration CreateRegistration()
        {
            return new MockRegistration();
        }

        public static PushConfig GetConfig()
        {
            return ReadConfig();
        }

    }

    public class MockRegistration : Registration
    {
        protected override Installation CreateInstallation(PushConfig pushConfig)
        {
            return new Installation();
        }

        protected override ILocalStore CreateChannelStore()
        {
            return new LocalStore();
        }

        public override Task<PushConfig> LoadConfigJson(string filename)
        {
            return Task.Run(() => new PushConfig());
        }

        protected override Task<string> ChannelUri()
        {
            return Task.Run(() => "http://dummy-channel");
        }
    }
}
