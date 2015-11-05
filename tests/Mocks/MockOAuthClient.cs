using System.Threading.Tasks;
using FHSDK.Services.Auth;

namespace tests.Mocks
{
    internal class MockOAuthClient : IOAuthClientHandlerService
    {
        public Task<OAuthResult> Login(string oauthLoginUrl)
        {
            return Task.Factory.StartNew(() => new OAuthResult(OAuthResult.ResultCode.Ok, "token", "http://login.url"));
        }
    }
}