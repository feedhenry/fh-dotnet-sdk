using System;
using System.Threading.Tasks;
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
            Uri uri = new Uri(oauthLoginUrl, UriKind.Absolute);
            WebAuthenticationBroker.AuthenticateAndContinue(uri);
            return null;
        }
    }
}
