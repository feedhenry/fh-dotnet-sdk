using System.Threading.Tasks;
using FHSDK.Services;
using FHSDK.Services.Hash;
using FHSDK.Sync;
using FHSDKPortable;
using Newtonsoft.Json.Linq;
using Xunit;

namespace tests
{
    public class HashTester
    {
        [Fact]
        public async Task TestStringHash()
        {
            //given
            await FHClient.Init();
            const string text = "test";
            var hasher = ServiceFinder.Resolve<IHashService>();

            //when
            var nativeHashed = hasher.GenerateSha1Hash(text);

            Assert.Equal("a94a8fe5ccb19ba61c4c0873d391e987982fbbd3", nativeHashed);
        }

        [Fact]
        public async Task TestObjectHash()
        {
            //given
            await FHClient.Init();
            var testObject = new JObject();
            testObject["testKey"] = "Test Data";
            testObject["testBoolKey"] = true;
            testObject["testNumKey"] = 10;
            var arr = new JArray { "obj1", "obj2" };
            testObject["testArrayKey"] = arr;
            var obj = new JObject();
            obj["obj3key"] = "obj3";
            obj["obj4key"] = "obj4";
            testObject["testDictKey"] = obj;

            //when
            var hash = FHSyncUtils.GenerateSHA1Hash(testObject);

            //then
            Assert.Equal("5f4675723d658919ede35fac62fade8c6397df1d", hash);
        }

        [Fact]
        public void TestGenerateHashWithUnderscoreInKey()
        {
            // given
            var data = new JObject();
            data["COMMENTS"] = "";
            data["FHID"] = "2553C7ED-9025-48F9-A346-EBE3E3AF943B";
            data["QUESTION_ID"] = 22;
            data["QUES_VALUE"] = "NO";
            data["VISIT_ID"] = 100220;
            data["TEST1_ttt"] = "test";
            data["TEST11_ttt"] = "test2";

            // when
            var hash = FHSyncUtils.GenerateSHA1Hash(data);

            // then
            Assert.Equal("824d6ded431d16fe8f2ab02b0744ca06822a3fff", hash);
        }
    }
}
