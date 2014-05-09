using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FHSDK.Services
{
    /// <summary>
    /// Class to handle oAuth logins given the oauth login url
    /// </summary>
    public interface IOAuthClientHandlerService
    {
        Task<OAuthResult> Login(string oauthLoginUrl);
    }

	public abstract class OAuthClientHandlerServiceBase: IOAuthClientHandlerService
	{
		public OAuthClientHandlerServiceBase()
		{

		}

		protected void OnSuccess(Uri uri, TaskCompletionSource<OAuthResult> tcs)
		{
			string queryParams = uri.Query;
			string[] parts = queryParams.Split('&');
			Dictionary<string, string> queryMap = new Dictionary<string, string>();
			for (int i = 0; i < parts.Length; i++)
			{
				string[] kv = parts[i].Split('=');
				queryMap.Add(kv[0], kv[1]);
			}

			string result = null;
			queryMap.TryGetValue("result", out result);
			if ("success" == result)
			{
				string sessionToken = null;
				string authRes = null;
				queryMap.TryGetValue("fh_auth_session", out sessionToken);
				queryMap.TryGetValue("authResponse", out authRes);
				OAuthResult oauthResult = new OAuthResult(OAuthResult.ResultCode.OK, sessionToken, Uri.UnescapeDataString(authRes));
				tcs.TrySetResult(oauthResult);
			}
			else
			{
				string errorMessage = null;
				queryMap.TryGetValue("message", out errorMessage);
				OAuthResult oauthResult = new OAuthResult(OAuthResult.ResultCode.FAILED, new Exception(errorMessage));
				tcs.TrySetResult(oauthResult);
			}

		}

		public	abstract Task<OAuthResult> Login(string oauthLoginUrl);
	}

    /// <summary>
    /// Class represents the result of the OAuth login
    /// </summary>
    public class OAuthResult
    {
        public enum ResultCode 
        {
            OK = 0,
            FAILED = 1,
            CANCELLED = 2,
            UNKNWON = -1
        }

        private string sessionToken = null;
        private string authResponse = null;
        private Exception exception = null;
        private ResultCode resultCode;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="result">the result code</param>
        public OAuthResult(ResultCode result)
        {
            this.resultCode = result;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="result">the result code</param>
        /// <param name="exception">the error exception</param>
        public OAuthResult(ResultCode result, Exception exception)
        {
            this.resultCode = result;
            this.exception = exception;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="result">the result code</param>
        /// <param name="sessionToken">the session token</param>
        /// <param name="authResponse">the acutal OAuth response</param>
        public OAuthResult(ResultCode result, string sessionToken, string authResponse)
        {
            this.resultCode = result;
            this.sessionToken = sessionToken;
            this.authResponse = authResponse;
        }

        /// <summary>
        /// Get the result code
        /// </summary>
        public ResultCode Result
        {
            get
            {
                return this.resultCode;
            }
        }

        /// <summary>
        /// Get the sessionToken
        /// </summary>
        public string SessionToken
        {
            get
            {
                return this.sessionToken;
            }
        }

        /// <summary>
        /// Get the acutal OAuth response
        /// </summary>
        public string AuthResponse
        {
            get
            {
                return this.authResponse;
            }
        }

        /// <summary>
        /// Get the error message if failed.
        /// </summary>
        public Exception Error
        {
            get
            {
                return this.exception;
            }
        }

        /// <summary>
        /// Return the string representation of this object
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            JObject resJson = new JObject();
            resJson["sessionToken"] = this.SessionToken;
            resJson["authResponse"] = this.AuthResponse;
            return resJson.ToString();
        }
        
    }
}
