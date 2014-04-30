using FHSDK.Services;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using FHSDK.API;
using FHSDK.FHHttpClient;
using System.Diagnostics.Contracts;

namespace FHSDK
{
    /// <summary>
    /// This is the main FeedHenry SDK class
    /// </summary>
	public abstract class FHBase
    {
        const double DEFAULT_TIMEOUT = 30 * 1000;
		protected static bool appReady = false;
		protected static CloudProps cloudProps = null;
		protected static TimeSpan timeout = TimeSpan.FromMilliseconds(DEFAULT_TIMEOUT);
		const string SDK_VERSION_STRING = "1.0.0.0";


        /// <summary>
        /// Get the current version of the FeedHenry WindowsPhone SDk
        /// </summary>
		public static string SDK_VERSION
        {
            get
            {
				return SDK_VERSION_STRING;
            }
        }

        /// <summary>
        /// Get or Set the timeout value for all the requests. Default is 30 seconds.
        /// </summary>
		protected static TimeSpan TimeOut
        {
            get
            {
                return timeout;
            }

            set
            {
                timeout = value;
            }
        }

        /// <summary>
        /// Initialise FeedHenry WindowsPhone SDK. This function should be called when the app is ready.
        /// </summary>
        /// <example>
        /// <code>
        /// try
        /// {
        ///   bool inited = await FH.Init();
        ///   if(inited)
        ///   {
        ///     //Initialisation is successful
        ///   }
        /// }
        /// catch(FHException e)
        /// {
        ///   //Initialisation failed, handle exception
        /// }
        /// </code>
        /// </example>
        /// <returns>If Init is success or not</returns>
        /// <exception cref="FHException"></exception>
		protected static async Task<bool> Init()
        {
			FHConfig.getInstance();
            if (!appReady)
            {
                FHInitRequest initRequest = new FHInitRequest();
                initRequest.TimeOut = timeout;
                FHResponse initRes = await initRequest.execAsync();
                if (null == initRes.Error)
                {
					JObject resJson = initRes.GetResponseAsJObject();
					cloudProps = new CloudProps (resJson);
                    appReady = true;
					JToken initValue = resJson["init"];
                    if (null != initValue)
                    {
                        SaveInitInfo(initValue.ToString());
                    }
                    return true;
                }
                else
                {
                    throw initRes.Error;
                }
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// Invoke a cloud function.
        /// </summary>
        /// <param name="remoteAct">The name of the cloud function name</param>
        /// <param name="actParams">The parameters passed to the cloud function</param>
        /// <example>
        /// <code>
        /// string cloudFunc = "test";
        /// IDictionary&lt;string, object&gt; dict = new Dictionary&lt;string, object&gt;();
        /// dict.Add("data", "test");
        /// FHResponse response = await FH.Act(cloudFunc, dict);
        /// if(null == response.Error)
        /// {
        ///   //no error occured, the request is successful
        ///   string rawResponseData = response.RawResponse;
        ///   //you can get it as JSONObject (require Json.Net library)
        ///   JObject resJson = response.GetResponseAsJObject();
        ///   //process response data
        /// }
        /// else
        /// {
        ///   //error occured during the request, deal with it.
        ///   //More infomation can be access from response.Error.InnerException
        /// }
        /// </code>
        /// </example>
        /// <returns>The response data returned by the cloud function</returns>
        /// <exception cref="InvalidOperationException"> It will be thrown if FH SDK is not ready.</exception>
        public static async Task<FHResponse> Act(string remoteAct, IDictionary<string, object> actParams)
        {
            RequireAppReady();
            FHActRequest actRequest = new FHActRequest(cloudProps);
            actRequest.TimeOut = timeout;
            return await actRequest.execAsync(remoteAct, actParams);
        }

        public static async Task<FHResponse> Auth(string policyId)
        {
            RequireAppReady();
            FHAuthRequest authRequest = new FHAuthRequest();
            authRequest.TimeOut = timeout;
            authRequest.SetAuthPolicyId(policyId);
            return await authRequest.execAsync();
        }

        public static async Task<FHResponse> Auth(string policyId, string userName, string userPassword)
        {
            RequireAppReady();
            FHAuthRequest authRequest = new FHAuthRequest();
            authRequest.TimeOut = timeout;
            authRequest.SetAuthUser(policyId, userName, userPassword);
            return await authRequest.execAsync();
        }
			
		public static FHCloudRequest GetCloudRequest(string path, string requestMethod, IDictionary<string, string> headers, IDictionary<string, object> requestParams)
		{
			RequireAppReady ();
			Contract.Assert (null != path, "Cloud path is not defined");
			Contract.Assert (null != requestMethod, "Request method is not defined");
			FHCloudRequest cloudRequest = new FHCloudRequest (cloudProps);
			cloudRequest.RequestMethod = requestMethod;
			cloudRequest.RequestPath = path;
			cloudRequest.RequestHeaders = headers;
			cloudRequest.RequestParams = requestParams;
			return cloudRequest;
		}

		public static async Task<FHResponse> Cloud(string path, string requestMethod, IDictionary<string, string> headers, IDictionary<string, object> requestParams)
		{
			FHCloudRequest cloudRequest = GetCloudRequest (path, requestMethod, headers, requestParams);
			return await cloudRequest.execAsync ();
		}

		public static string GetCloudHost()
		{
			RequireAppReady ();
			return cloudProps.GetCloudHost ();
		}

		public static IDictionary<string, object> GetDefaultParams()
		{
			Dictionary<string, object> defaults = new Dictionary<string, object>();
			FHConfig appConfig = FHConfig.getInstance ();
			defaults ["appid"] = appConfig.GetAppId ();
			defaults ["appkey"] = appConfig.GetAppKey ();
			defaults ["cuid"] = appConfig.GetDeviceId ();
			defaults ["destination"] = appConfig.GetDestination ();
			defaults["sdk_version"] = "FH_SDK/" + SDK_VERSION;
			if (null != appConfig.GetProjectId())
			{
				defaults["projectid"] = appConfig.GetProjectId();
			}
			if (null != appConfig.GetConnectionTag()) {
				defaults["connectiontag"] = appConfig.GetConnectionTag();
			}
			JObject initInfo = GetInitInfo();
			if (null != initInfo)
			{
				defaults["init"] = initInfo;
			}
			return defaults;
		}

		public static IDictionary<string, string> GetDefaultParamsAsHeaders()
		{
			IDictionary<string, object> defaultParams = GetDefaultParams ();
			IDictionary<string, string> headers = new Dictionary<string, string> ();
			foreach (var item in defaultParams) {
				string headername = "X-FH-" + item.Key;
				headers.Add (headername, JsonConvert.SerializeObject(item.Value));
			}
			return headers;
		}
        

        /// <summary>
        /// Save app init info. Mainly used for analytics.
        /// </summary>
        /// <param name="initInfo"></param>
		protected static void SaveInitInfo(string initInfo)
        {
			IDataService dataService = GetDataService();
			dataService.SaveData("init", initInfo);
        }

		/// <summary>
		/// </summary>
		/// <returns></returns>
		protected static JObject GetInitInfo()
		{
			IDataService dataService = GetDataService();
			string initValue = dataService.GetData("init");
			if (null != initValue) {
				return JObject.Parse (initValue);
			}
			return null;
		}

		private static IDataService GetDataService()
		{
			return (IDataService) ServiceFinder.Resolve<IDataService>();
		}

		private static void RequireAppReady()
		{
			Contract.Assert (appReady, "FH is not ready. Have you called FH.Init?");
		}

    }

    


}
