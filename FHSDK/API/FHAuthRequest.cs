using System;
using FHSDK.FHHttpClient;
using FHSDK.Services;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Threading.Tasks;

namespace FHSDK.API
{
	public class FHAuthRequest : FHRequest
	{
		const string AUTH_PATH = "box/srv/1.1/admin/authpolicy/auth";
		private string authPolicyId;
		private string authUserName;
		private string authUserPass;
		private IOAuthClientHandlerService oauthClient = null;

		public FHAuthRequest()
			: base()
		{
			this.oauthClient = ServiceFinder.Resolve<IOAuthClientHandlerService>();
		}

		public void SetAuthPolicyId(string authPolicy)
		{
			this.authPolicyId = authPolicy;
		}

		public void SetAuthUser(string authPolicy, string authUserName, string authPassword)
		{
			this.authPolicyId = authPolicy;
			this.authUserName = authUserName;
			this.authUserPass = authPassword;
		}

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
		protected override IDictionary<string, object> GetRequestParams()
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
			IDictionary<string, object> defaultParams = GetDefaultParams();
			data["__fh"] = defaultParams;
			return data;
		}

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
						resData.TryGetValue("url", out oauthurl);
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

