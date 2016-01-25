
using System;
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
            Assert.AreEqual(new Uri("http://192.168.28.34:8001/api/v2/ag-push"), config.UnifiedPushUri);
            Assert.IsNotNull (config.Categories);
            Assert.AreEqual (2, config.Categories.Count);
            Assert.IsTrue (config.Categories.IndexOf ("one") != -1);
            Assert.IsTrue (config.Categories.IndexOf ("two") != -1);
        }
    }
}
