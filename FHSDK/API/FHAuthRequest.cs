using System;
using FHSDK.FHHttpClient;
using FHSDK.Services;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Threading.Tasks;

namespace FHSDK.API
{
    /// <summary>
    /// Class represents an authentication API request
    /// </summary>
	public class FHAuthRequest : FHRequest
	{
		const string AUTH_PATH = "box/srv/1.1/admin/authpolicy/auth";
		private string authPolicyId;
		private string authUserName;
		private string authUserPass;
		private IOAuthClientHandlerService oauthClient = null;

        private CloudProps cloudProps;

        /// <summary>
        /// Constructor
        /// </summary>
		public FHAuthRequest(CloudProps cloudProps)
			: base()
		{
            this.cloudProps = cloudProps;
			this.oauthClient = ServiceFinder.Resolve<IOAuthClientHandlerService>();
		}

        /// <summary>
        /// Set the policy id for the request
        /// </summary>
        /// <param name="authPolicy">the auth policy id</param>
		public void SetAuthPolicyId(string authPolicy)
		{
			this.authPolicyId = authPolicy;
		}

        /// <summary>
        /// Set the policy id and user credentials for the request
        /// </summary>
        /// <param name="authPolicy">the auth policy id</param>
        /// <param name="authUserName">the auth user name</param>
        /// <param name="authPassword">the auth user password</param>
		public void SetAuthUser(string authPolicy, string authUserName, string authPassword)
		{
			this.authPolicyId = authPolicy;
			this.authUserName = authUserName;
			this.authUserPass = authPassword;
		}

        /// <summary>
        /// Set the OAuth handler. The auth API will return a URL to redirect users to login for OAuth type authentications.
        /// The handler need to implement the function to allow user to login and return the authentication info at the end.
        /// </summary>
        /// <param name="oauthHandler">the handler for OAuth login</param>
		public void SetOAuthHandler(IOAuthClientHandlerService oauthHandler)
		{
			this.oauthClient = oauthHandler;
		}

		/// <summary>
		/// Construct the remote uri based on the request type
		/// </summary>
		/// <returns></returns>
		protected override Uri GetUri()
		{
			return new Uri(String.Format("{0}/{1}", appConfig.GetHost(), AUTH_PATH));
		}

		/// <summary>
		/// Construct the request data based on the request type
		/// </summary>
		/// <returns></returns>
		protected override object GetRequestParams()
		{

			Dictionary<string, object> data = new Dictionary<string, object>();
			if (null == this.authPolicyId)
			{
				throw new ArgumentNullException("No auth policy id");
			}
			data.Add("policyId", this.authPolicyId);
			data.Add("device", appConfig.GetDeviceId());
			data.Add("clientToken", appConfig.GetAppId());
			Dictionary<string, string> userParams = new Dictionary<string, string>();
			if (null != this.authUserName && null != this.authUserPass)
			{
				userParams.Add("userId", this.authUserName);
				userParams.Add("password", this.authUserPass);
			}
			data.Add("params", userParams);
            string env = this.cloudProps.GetEnv();
            if (null != env)
            {
                data.Add("environment", env);
            }
			IDictionary<string, object> defaultParams = GetDefaultParams();
			data["__fh"] = defaultParams;
			return data;
		}

        /// <summary>
        /// Execute the authentication request. If the authencation type is OAuth and an OAuthHandler is set, it will be called automatically to redirect users to login.
        /// </summary>
        /// <returns>the authentication details</returns>
		public override async Task<FHResponse> execAsync()
		{
			FHResponse fhres = await base.execAsync();
			if (null == this.oauthClient)
			{
				return fhres;
			}
			else
			{
				if (null == fhres.Error)
				{
					JObject resData = fhres.GetResponseAsJObject();
					string status = (string)resData["status"];
					if ("ok" == status)
					{
						JToken oauthurl = null;
                        JToken sessionToken = null;
						resData.TryGetValue("url", out oauthurl);
                        resData.TryGetValue("sessionToken", out sessionToken);
                        if (null != sessionToken)
                        {
                            FHAuthSession authSession = FHAuthSession.Instance;
                            authSession.SaveSession((string)sessionToken);
                        }
						if (null == oauthurl)
						{
							return fhres;
						}
						else
						{
							OAuthResult oauthLoginResult = await this.oauthClient.Login((string)oauthurl);
							FHResponse authRes = null;
							if (oauthLoginResult.Result == OAuthResult.ResultCode.OK)
							{
								authRes = new FHResponse(HttpStatusCode.OK, oauthLoginResult.ToString());
							}
							else if (oauthLoginResult.Result == OAuthResult.ResultCode.FAILED)
							{
								authRes = new FHResponse(null, new FHException("Authentication Failed. Message = " + oauthLoginResult.Error.Message, FHException.ErrorCode.AuthenticationError, oauthLoginResult.Error));
							}
							else if (oauthLoginResult.Result == OAuthResult.ResultCode.CANCELLED)
							{
								authRes = new FHResponse(null, new FHException("Cancelled", FHException.ErrorCode.Cancelled));
							}
							else
							{
								authRes = new FHResponse(null, new FHException("Unknown Error", FHException.ErrorCode.UnknownError, oauthLoginResult.Error));
							}
							return authRes;
						}
					}
					else
					{
						FHResponse authRes = new FHResponse(HttpStatusCode.BadRequest, fhres.RawResponse, new FHException("Authentication Failed", FHException.ErrorCode.AuthenticationError));
						return authRes;
					}
				}
				else
				{
					return fhres;
				}
			}
		}


	}
}

