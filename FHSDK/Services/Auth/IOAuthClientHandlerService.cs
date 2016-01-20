using System;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace FHSDK.Services.Auth
{
    /// <summary>
    /// Interface to handle OAuth login.
    /// </summary>
    public interface IOAuthClientHandlerService
    {
        Task<OAuthResult> Login(string oauthLoginUrl);
    }
    /// <summary>
    ///  Class to handle OAuth login given the oauth login URL.
    /// </summary>
    public abstract class OAuthClientHandlerServiceBase : IOAuthClientHandlerService
    {
        /// <summary>
        /// Abstract method to login.
        /// </summary>
        /// <param name="oauthLoginUrl"></param>
        /// <returns></returns>
        public abstract Task<OAuthResult> Login(string oauthLoginUrl);

        /// <summary>
        /// Callback method once the login method is successfull.
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="tcs"></param>
        protected static void OnSuccess(Uri uri, TaskCompletionSource<OAuthResult> tcs)
        {
            var queryParams = uri.Query;
            var parts = queryParams.Split('&');
            var queryMap = parts.Select(t => t.Split('=')).ToDictionary(kv => kv[0], kv => kv[1]);

            string result;
            queryMap.TryGetValue("result", out result);
            if ("success" == result)
            {
                string sessionToken;
                string authRes;
                queryMap.TryGetValue("fh_auth_session", out sessionToken);
                queryMap.TryGetValue("authResponse", out authRes);
                var oauthResult = new OAuthResult(OAuthResult.ResultCode.Ok, sessionToken,
                    Uri.UnescapeDataString(authRes));
                tcs.TrySetResult(oauthResult);
            }
            else
            {
                string errorMessage;
                queryMap.TryGetValue("message", out errorMessage);
                var oauthResult = new OAuthResult(OAuthResult.ResultCode.Failed, new Exception(errorMessage));
                tcs.TrySetResult(oauthResult);
            }
        }
    }

    /// <summary>
    /// Class represents the result of the OAuth login.
    /// </summary>
    public class OAuthResult
    {
        /// <summary>
        /// Enum to list possible OAuth result.
        /// </summary>
        public enum ResultCode
        {
            Ok = 0,
            Failed = 1,
            Cancelled = 2,
            Unknwon = -1
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="result">the result code</param>
        public OAuthResult(ResultCode result)
        {
            Error = null;
            AuthResponse = null;
            SessionToken = null;
            Result = result;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="result">the result code</param>
        /// <param name="exception">the error exception</param>
        public OAuthResult(ResultCode result, Exception exception)
        {
            AuthResponse = null;
            SessionToken = null;
            Result = result;
            Error = exception;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="result">the result code</param>
        /// <param name="sessionToken">the session token</param>
        /// <param name="authResponse">the acutal OAuth response</param>
        public OAuthResult(ResultCode result, string sessionToken, string authResponse)
        {
            Error = null;
            Result = result;
            SessionToken = sessionToken;
            AuthResponse = authResponse;
        }

        /// <summary>
        /// Get the result code.
        /// </summary>
        public ResultCode Result { get; private set; }

        /// <summary>
        /// Get the sessionToken.
        /// </summary>
        public string SessionToken { get; private set; }

        /// <summary>
        /// Get the acutal OAuth response.
        /// </summary>
        public string AuthResponse { get; private set; }

        /// <summary>
        /// Get the error message if failed.
        /// </summary>
        public Exception Error { get; private set; }

        /// <summary>
        /// Return the string representation of this object.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            var resJson = new JObject();
            resJson["sessionToken"] = SessionToken;
            resJson["authResponse"] = AuthResponse;
            return resJson.ToString();
        }
    }
}