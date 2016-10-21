#if __MOBILE__
using Xunit;
using TestMethod = Xunit.FactAttribute;
#else
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
#endif

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FHSDK;
using FHSDK.FHHttpClient;
using Newtonsoft.Json.Linq;
using tests.Mocks;

namespace tests
{
    [TestClass]
    public class HttpClientTest
    {
        [TestMethod]
        public async Task ShouldSendAsync()
        {
            //given
            var mock = await InitMock();
            const string method = "POST";

            //when
            await FHHttpClient.SendAsync(new Uri("http://localhost/test"), method, new Dictionary<string, string>() {{"key", "value"}}, 
                "request-data", TimeSpan.FromSeconds(20));

            //then
            Assert.IsNotNull(mock.Request);
            Assert.AreEqual(method, mock.Request.Method.Method);
            Assert.IsTrue(mock.Request.Headers.Contains("key"));
            Assert.AreEqual("\"request-data\"", await mock.Request.Content.ReadAsStringAsync());
            Assert.AreEqual(20, mock.Timeout.Seconds);
        }

        [TestMethod]
        public async Task ShouldPerformGet()
        {
            //given
            var mock = await InitMock();
            const string method = "GET";

            //when
            await FHHttpClient.SendAsync(new Uri("http://localhost/test"), method, null,
                JObject.Parse("{'key-data': 'value'}"), TimeSpan.FromSeconds(20));

            //then
            Assert.IsNotNull(mock.Request);
            Assert.AreEqual("http://localhost/test?key-data=value", mock.Request.RequestUri.ToString());
        }

        [TestMethod]
        public async Task ShouldPerformGetWithString()
        {
            //given
            var mock = await InitMock();
            const string method = "GET";

            //when
            await FHHttpClient.SendAsync(new Uri("http://localhost/test"), method, null, 
                new Dictionary<string, object> { {"key1", "value1"}, {"key2", new Dictionary<string, string> { { "key3", "value3"}} } }, TimeSpan.FromSeconds(20));

            //then
            Assert.IsNotNull(mock.Request);
            Assert.AreEqual("http://localhost/test?key1=value1&key2[key3]=value3", mock.Request.RequestUri.ToString());
        }

        [TestMethod]
        public async Task ShouldPerformGetWithArray()
        {
            //given
            var mock = await InitMock();
            const string method = "GET";

            //when
            await FHHttpClient.SendAsync(new Uri("http://localhost/test"), method, null,
                new Dictionary<string, object> { { "key1", "value1" }, { "array", new List<object> { "one", "two", "three", new List<string> { "bis", "ter"} } } }, TimeSpan.FromSeconds(20));

            //then
            Assert.IsNotNull(mock.Request);
            Assert.AreEqual("http://localhost/test?key1=value1&array[]=one&array[]=two&array[]=three&array[][]=bis&array[][]=ter", mock.Request.RequestUri.ToString());
        }

        private static async Task<MockHttpClient> InitMock()
        {
            await FHClient.Init();
            var mock = new MockHttpClient();
            FHHttpClientFactory.Get = () => mock;
            return mock;
        }
    }
}