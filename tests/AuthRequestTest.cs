#if __MOBILE__
using Xunit;
using TestMethod = Xunit.FactAttribute;
#else
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
#endif

using System.Threading.Tasks;
using FHSDK;
using FHSDK.API;
using FHSDK.Config;
using FHSDK.FHHttpClient;
using FHSDK.Services;
using FHSDK.Services.Auth;
using Newtonsoft.Json.Linq;
using tests.Mocks;

namespace tests
{
    [TestClass]
    public class AuthRequestTest
    {
        [TestMethod]
        public async Task ShouldOAuthRequest()
        {
			await FHClient.Init();

            //given
            ServiceFinder.RegisterType<IOAuthClientHandlerService, MockOAuthClient>();

            var json = JObject.Parse(@"{
                ""hosts"": {""host"": ""HOST""}
             }");
            var config = new FHConfig(new MockDeviceService());
            var props = new CloudProps(json, config);
            var authRequest = new FHAuthRequest(props);

            authRequest.SetAuthUser("gmail", "user", "password");

            var mockHttpCLient =
                new MockHttpClient {Content = "{\"status\": \"ok\", \"url\": \"http://oauthurl.url\"}"};
            FHHttpClientFactory.Get = () => mockHttpCLient;

            //when
            var response = await authRequest.ExecAsync();

            //then
            Assert.IsNotNull(response);
            var responseParams = response.GetResponseAsDictionary();
            Assert.IsNotNull(responseParams);
            Assert.AreEqual("token", responseParams["sessionToken"]);
        }
    }
}
