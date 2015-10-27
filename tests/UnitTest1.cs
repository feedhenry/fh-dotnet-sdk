using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using FHSDK;
using FHSDK.Services;
using FHSDK.Services.Device;
using FHSDK.Services.Hash;
using FHSDK.Sync;
using FHSDKPortable;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;

namespace tests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public async Task TestInit()
        {
            //given
            await FHClient.Init();

            //when
            var host = FH.GetCloudHost();

            //then
            Assert.True(host.Contains("http://192.168.28.34:8001"));
        }
    }
}
