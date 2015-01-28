using FHSDK.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using Windows.Security.Authentication.Web;

namespace FHSDK.Services
{
    /// <summary>
    /// OAuth login handler for windows phone
    /// </summary>
    class OAuthClientHandlerService : OAuthClientHandlerServiceBase
    {
        private TaskCompletionSource<OAuthResult> tcs;

        /// <summary>
        /// Constructor
        /// </summary>
        public OAuthClientHandlerService()
            : base()
        {

        }

        public override async Task<OAuthResult> Login(string oauthLoginUrl)
        {
            tcs = new TaskCompletionSource<OAuthResult>();
            Uri uri = new Uri(oauthLoginUrl, UriKind.Absolute);
            var oauth = await WebAuthenticationBroker.AuthenticateAsync(WebAuthenticationOptions.None, uri);
            if (oauth.ResponseStatus == WebAuthenticationStatus.Success)
            {
                base.OnSuccess(new Uri(oauth.ResponseData), tcs);
            }
            else if (oauth.ResponseStatus == WebAuthenticationStatus.UserCancel)
            {
                tcs.SetResult(new OAuthResult(OAuthResult.ResultCode.CANCELLED));
            }
            else
            {
                tcs.SetResult(new OAuthResult(OAuthResult.ResultCode.FAILED));
            }

            return await tcs.Task;
        }
    }
}
