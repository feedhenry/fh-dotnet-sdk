using System;
using System.Threading.Tasks;
using Windows.Security.Authentication.Web;

namespace FHSDK.Services.Auth
{
    /// <summary>
    ///     OAuth login handler for windows phone
    /// </summary>
    internal class OAuthClientHandlerService : OAuthClientHandlerServiceBase
    {
        private TaskCompletionSource<OAuthResult> _tcs;

        public override async Task<OAuthResult> Login(string oauthLoginUrl)
        {
            _tcs = new TaskCompletionSource<OAuthResult>();
            var uri = new Uri(oauthLoginUrl, UriKind.Absolute);
            var oauth = await WebAuthenticationBroker.AuthenticateAsync(WebAuthenticationOptions.None, uri);
            switch (oauth.ResponseStatus)
            {
                case WebAuthenticationStatus.Success:
                    OnSuccess(new Uri(oauth.ResponseData), _tcs);
                    break;
                case WebAuthenticationStatus.UserCancel:
                    _tcs.SetResult(new OAuthResult(OAuthResult.ResultCode.Cancelled));
                    break;
                default:
                    _tcs.SetResult(new OAuthResult(OAuthResult.ResultCode.Failed));
                    break;
            }

            return await _tcs.Task;
        }
    }
}