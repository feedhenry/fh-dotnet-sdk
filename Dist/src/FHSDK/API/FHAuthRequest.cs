using System;
using FHSDK.FHHttpClient;
using FHSDK.Services;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Threading.Tasks;
using FHSDK.Services.Auth;

namespace FHSDK.API
{
    /// <summary>
    /// Class represents an authentication API request
    /// </summary>
	public class FHAuthRequest : FHRequest
	{
		const string AuthPath = "box/srv/1.1/admin/authpolicy/auth";
		private string _authPolicyId;
		private string _authUserName;
		private string _authUserPass;
		private IOAuthClientHandlerService _oauthClient = null;

        private readonly CloudProps _cloudProps;

        /// <summary>
        /// Constructor
        /// </summary>
		public FHAuthRequest(CloudProps cloudProps)
        {
            _cloudProps = cloudProps;
			_oauthClient = ServiceFinder.Resolve<IOAuthClientHandlerService>();
		}

        /// <summary>
        /// Set the policy id for the request
        /// </summary>
        /// <param name="authPolicy">the auth policy id</param>
		public void SetAuthPolicyId(string authPolicy)
		{
			_authPolicyId = authPolicy;
		}

        /// <summary>
        /// Set the policy id and user credentials for the request
        /// </summary>
        /// <param name="authPolicy">the auth policy id</param>
        /// <param name="authUserName">the auth user name</param>
        /// <param name="authPassword">the auth user password</param>
		public void SetAuthUser(string authPolicy, string authUserName, string authPassword)
		{
			_authPolicyId = authPolicy;
			_authUserName = authUserName;
			_authUserPass = authPassword;
		}

        /// <summary>
        /// Set the OAuth handler. The auth API will return a URL to redirect users to login for OAuth type authentications.
        /// The handler need to implement the function to allow user to login and return the authentication info at the end.
        /// </summary>
        /// <param name="oauthHandler">the handler for OAuth login</param>
		public void SetOAuthHandler(IOAuthClientHandlerService oauthHandler)
		{
			_oauthClient = oauthHandler;
		}

		/// <summary>
		/// Construct the remote uri based on the request type
		/// </summary>
		/// <returns></returns>
		protected override Uri GetUri()
		{
			return new Uri(String.Format("{0}/{1}", AppConfig.GetHost(), AuthPath));
		}

		/// <summary>
		/// Construct the request data based on the request type
		/// </summary>
		/// <returns></returns>
		protected override object GetRequestParams()
		{

			var data = new Dictionary<string, object>();
			if (null == _authPolicyId)
			{
				throw new ArgumentNullException("No auth policy id");
			}
			data.Add("policyId", _authPolicyId);
			data.Add("device", AppConfig.GetDeviceId());
			data.Add("clientToken", AppConfig.GetAppId());
			Dictionary<string, string> userParams = new Dictionary<string, string>();
			if (null != _authUserName && null != this._authUserPass)
			{
				userParams.Add("userId", _authUserName);
				userParams.Add("password", _authUserPass);
			}
			data.Add("params", userParams);
            string env = _cloudProps.GetEnv();
            if (null != env)
            {
                data.Add("environment", env);
            }
			var defaultParams = GetDefaultParams();
			data["__fh"] = defaultParams;
			return data;
		}

        /// <summary>
        /// Execute the authentication request. If the authencation type is OAuth and an OAuthHandler is set, it will be called automatically to redirect users to login.
        /// </summary>
        /// <returns>the authentication details</returns>
		public override async Task<FHResponse> ExecAsync()
		{
			var fhres = await base.ExecAsync();
			if (null == this._oauthClient)
			{
				return fhres;
			}
			else
			{
                if (null != fhres.Error) return fhres;
			    var resData = fhres.GetResponseAsJObject();
			    var status = (string)resData["status"];
			    if ("ok" == status)
			    {
			        JToken oauthurl = null;
			        JToken sessionToken = null;
			        resData.TryGetValue("url", out oauthurl);
			        resData.TryGetValue("sessionToken", out sessionToken);
			        if (null != sessionToken)
			        {
			            var authSession = FHAuthSession.GetInstance;
			            FHAuthSession.SaveSession((string)sessionToken);
			        }
			        if (null == oauthurl)
			        {
			            return fhres;
			        }
			        var oauthLoginResult = await this._oauthClient.Login((string)oauthurl);
			        FHResponse authRes = null;
			        switch (oauthLoginResult.Result)
			        {
			            case OAuthResult.ResultCode.Ok:
			                authRes = new FHResponse(HttpStatusCode.OK, oauthLoginResult.ToString());
			                break;
			            case OAuthResult.ResultCode.Failed:
			                authRes = new FHResponse(null, new FHException("Authentication Failed. Message = " + oauthLoginResult.Error.Message, FHException.ErrorCode.AuthenticationError, oauthLoginResult.Error));
			                break;
			            case OAuthResult.ResultCode.Cancelled:
			                authRes = new FHResponse(null, new FHException("Cancelled", FHException.ErrorCode.Cancelled));
			                break;
			            default:
			                authRes = new FHResponse(null, new FHException("Unknown Error", FHException.ErrorCode.UnknownError, oauthLoginResult.Error));
			                break;
			        }
			        return authRes;
			    }
			    else
			    {
			        var authRes = new FHResponse(HttpStatusCode.BadRequest, fhres.RawResponse, new FHException("Authentication Failed", FHException.ErrorCode.AuthenticationError));
			        return authRes;
			    }
			}
		}


	}
}

