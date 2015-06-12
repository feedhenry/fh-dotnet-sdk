#if __ANDROID__
using FHSDK.Droid;
#elif __IOS__
using FHSDK.Touch;
#endif
using System.Collections.Generic;
using System.Threading.Tasks;
using FHSDK;
using Newtonsoft.Json.Linq;
#if WINDOWS_PHONE
using TestFixture = Microsoft.VisualStudio.TestPlatform.UnitTestFramework.TestClassAttribute;
using Test = Microsoft.VisualStudio.TestPlatform.UnitTestFramework.TestMethodAttribute;
using SetUp = Microsoft.VisualStudio.TestPlatform.UnitTestFramework.TestInitializeAttribute;
using TearDown = Microsoft.VisualStudio.TestPlatform.UnitTestFramework.TestCleanupAttribute;
using FHSDK.Phone;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
#else
using NUnit.Framework;
#endif

namespace FHSDKTestShared
{
    [TestFixture]
    public class FHSDKTest
    {
        [SetUp]
        public void Setup()
        {
            FHClient.Init();
        }

        [Test]
        public void TestInit()
        {
            var host = FH.GetCloudHost();
            Assert.IsTrue(host.Contains("http://192.168.28.34:8001"));
        }

        [Test]
#if WINDOWS_PHONE
        public async Task TestAct()
#else
        public async void TestAct()
        #endif
        {
            var data = new Dictionary<string, string>();
            data["test"] = "test";
            var actRes = await FH.Act("echo", data);
            var resJson = actRes.GetResponseAsJObject();
            Assert.IsNotNull(resJson["method"]);
            Assert.IsNotNull(resJson["headers"]);
            Assert.IsNotNull(resJson["body"]);
            Assert.IsTrue("POST".Equals((string) resJson["method"]));
            var headers = (JObject) resJson["headers"];
            Assert.IsNotNull(headers["x-fh-cuid"]);
            var body = (JObject) resJson["body"];
            Assert.IsNotNull(body["__fh"]);
            Assert.IsNotNull(body["test"]);
            Assert.IsTrue("test".Equals((string) body["test"]));
        }

        [Test]
#if WINDOWS_PHONE
        public async Task TestCloud()
#else
        public async void TestCloud()
        #endif
        {
            await TestCloud("GET");
            await TestCloud("POST");
        }

        private async Task TestCloud(string method)
        {
            var headers = new Dictionary<string, string>();
            headers["x-test-header"] = "testheader";
            var data = new Dictionary<string, string>();
            data["test"] = "test";
            var cloudRes = await FH.Cloud("echo", method, headers, data);
            var cloudResJson = cloudRes.GetResponseAsJObject();
            Assert.IsNotNull(cloudResJson["method"]);
            Assert.IsTrue(method.ToUpper().Equals((string) cloudResJson["method"]));

            Assert.IsNotNull(cloudResJson["headers"]);
            Assert.IsNotNull(cloudResJson["body"]);

            var resHeaders = (JObject) cloudResJson["headers"];
            Assert.IsNotNull(resHeaders["x-fh-cuid"]);
            Assert.IsNotNull(resHeaders["x-test-header"]);


            Assert.IsTrue("testheader".Equals((string) resHeaders["x-test-header"]));
            JObject resData = null;
            if ("GET".Equals(method))
            {
                Assert.IsNotNull(cloudResJson["query"]);
                resData = (JObject) cloudResJson["query"];
            }
            else
            {
                Assert.IsNotNull(cloudResJson["body"]);
                resData = (JObject) cloudResJson["body"];
            }

            Assert.IsNotNull(resData["test"]);
            Assert.IsTrue(((string) resData["test"]).Contains("test"));
        }
    }
}