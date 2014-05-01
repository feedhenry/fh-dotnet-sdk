using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FHSDK.Services
{
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

        public OAuthResult(ResultCode result)
        {
            this.resultCode = result;
        }

        public OAuthResult(ResultCode result, Exception exception)
        {
            this.resultCode = result;
            this.exception = exception;
        }

        public OAuthResult(ResultCode result, string sessionToken, string authResponse)
        {
            this.resultCode = result;
            this.sessionToken = sessionToken;
            this.authResponse = authResponse;
        }

        public ResultCode Result
        {
            get
            {
                return this.resultCode;
            }
        }

        public string SessionToken
        {
            get
            {
                return this.sessionToken;
            }
        }

        public string AuthResponse
        {
            get
            {
                return this.authResponse;
            }
        }

        public Exception Error
        {
            get
            {
                return this.exception;
            }
        }

        public override string ToString()
        {
            JObject resJson = new JObject();
            resJson["sessionToken"] = this.SessionToken;
            resJson["authResponse"] = this.AuthResponse;
            return resJson.ToString();
        }
        
    }
}
