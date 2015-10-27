using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using FHSDK;
using FHSDK.Services;
using FHSDK.Services.Device;
using FHSDK.Services.Hash;
using FHSDK.Sync;
using FHSDKPortable;
using Newtonsoft.Json.Linq;
using Xunit;

namespace tests
{
    public class UnitTest1
    {
        [Fact]
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
