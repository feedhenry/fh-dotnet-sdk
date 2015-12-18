
using System.Collections.Generic;
#if __MOBILE__
using NUnit.Framework;
using TestClass = NUnit.Framework.TestFixtureAttribute;
using TestMethod = NUnit.Framework.TestAttribute;
#else
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
#endif

using System.Threading.Tasks;
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
            var config = PushBase.ReadConfig();

            //then
            Assert.AreEqual("http://192.168.28.34:8001/api/v2/ag-push", config.UnifiedPushUri);
            Assert.AreEqual(new List<string>() {"one", "two"}, config.Categories);
        }
    }
}
