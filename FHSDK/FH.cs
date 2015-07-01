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
using FHSDK.Services.Network;
using AeroGear.Push;

namespace FHSDK
{
    /// <summary>
    /// The parent name space defined by FeedHenry .Net SDK. It is defined inside the FHSDK.dll assembly, which is a Portable Class Library.
    /// The FHSDK.dll assembly can be referenced by other PCL projects.
    /// </summary>
    [System.Runtime.CompilerServices.CompilerGenerated]
    class NamespaceDoc
    {

    }
    /// <summary>
    /// This is the main FeedHenry SDK class
    /// </summary>
	public class FH
    {
        const double DEFAULT_TIMEOUT = 30 * 1000;
		protected static bool appReady = false;
		protected static CloudProps cloudProps = null;
		protected static TimeSpan timeout = TimeSpan.FromMilliseconds(DEFAULT_TIMEOUT);
		const string SDK_VERSION_STRING = "1.3.0";


        /// <summary>
        /// Get the current version of the FeedHenry .NET SDk
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
        /// The actual implementation of initialising the FeedHenry SDK. It is called when the Init method of each platform's FHClient class called in. 
        /// This way it will guarantee the platform's specific assembly will be loaded so that the ServiceFinder can find the correct implmenetation for some of the services.
        /// (The Adaptation approach used here works for wp and xamarain android without the FHClient reference. However, due to Xamarain IOS is using AOT compiler, we have to reference the FHClient class of the IOS SDK to make sure it will be loaded during compile.)
        /// </summary>
        /// <returns>If Init is success or not</returns>
        /// <exception cref="FHException"></exception>
		protected static async Task<bool> Init()
        {
			FHConfig fhconfig = FHConfig.getInstance();
            if (!appReady)
            {
                if(fhconfig.IsLocalDevelopment){
                    appReady = true;
                    JObject cloudJson = new JObject();
                    cloudJson["url"] = fhconfig.GetHost();
                    cloudProps = new CloudProps(cloudJson);
                    return true;
                }
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
        /// Invoke a cloud function which you have defined in cloud/main.js (the old way).
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
        public static async Task<FHResponse> Act(string remoteAct, object actParams)
        {
            RequireAppReady();
            FHActRequest actRequest = new FHActRequest(cloudProps);
            actRequest.TimeOut = timeout;
            return await actRequest.execAsync(remoteAct, actParams);
        }

        /// <summary>
        /// Call the FeedHenry Authentication API with the given policyId. This is normally used for OAuth type authentications. 
        /// The user will be prompted for login details and the the login result will be returned.
        /// </summary>
        /// <param name="policyId">The id of the new policy</param>
        /// <returns>The result of the authencation</returns>
        public static async Task<FHResponse> Auth(string policyId)
        {
            RequireAppReady();
            FHAuthRequest authRequest = new FHAuthRequest(cloudProps);
            authRequest.TimeOut = timeout;
            authRequest.SetAuthPolicyId(policyId);
            return await authRequest.execAsync();
        }

        /// <summary>
        ///  Call the FeedHenry Authentication API with the given policyId, user name and password. This is normally used for LDAP and other basic authentication types.
        /// </summary>
        /// <param name="policyId">The id of the auth policy</param>
        /// <param name="userName">The name of the user</param>
        /// <param name="userPassword">The user's password</param>
        /// <returns>The result of the authencation</returns>
        public static async Task<FHResponse> Auth(string policyId, string userName, string userPassword)
        {
            RequireAppReady();
            FHAuthRequest authRequest = new FHAuthRequest(cloudProps);
            authRequest.TimeOut = timeout;
            authRequest.SetAuthUser(policyId, userName, userPassword);
            return await authRequest.execAsync();
        }
		
	    /// <summary>
	    /// Build the cloud request to call the app's cloud functions.
	    /// </summary>
	    /// <param name="path">The path of the cloud request</param>
	    /// <param name="requestMethod">The request method</param>
	    /// <param name="headers">The HTTP headers for the request</param>
	    /// <param name="requestParams">The request body (will be covert to query parameters for certain request methods)</param>
	    /// <returns>The cloud request object</returns>
		public static FHCloudRequest GetCloudRequest(string path, string requestMethod, IDictionary<string, string> headers, object requestParams)
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

        /// <summary>
        /// Create a cloud request and execute it immediately.
        /// </summary>
        /// <param name="path">The path of the cloud request</param>
        /// <param name="requestMethod">The reqeust method</param>
        /// <param name="headers">The HTTP headers of the reqeust</param>
        /// <param name="requestParams">The request body (will be covert to query parameters for certain request methods)</param>
        /// <example>
        /// <code>
        /// FHResponse response = await FH.Cloud("api/echo", "GET", null, null);
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
        /// <returns>The response from the cloud</returns>
		public static async Task<FHResponse> Cloud(string path, string requestMethod, IDictionary<string, string> headers, object requestParams)
		{
			FHCloudRequest cloudRequest = GetCloudRequest (path, requestMethod, headers, requestParams);
			return await cloudRequest.execAsync ();
		}

        /// <summary>
        /// Invoke a FeedHenry MBAAS Service function
        /// </summary>
        /// <param name="service">The MBAAS service name</param>
        /// <param name="requestParams">The request body</param>
        /// <returns>The response from the MBAAS service</returns>
		public static async Task<FHResponse> Mbaas(string service, object requestParams)
		{
			RequireAppReady ();
			Contract.Assert (null != service, "service is not defined");
			string path = String.Format ("{0}/{1}", "mbaas", service);
			FHCloudRequest cloudRequest = GetCloudRequest (path, "POST", null, requestParams);
			return await cloudRequest.execAsync ();
		}

        /// <summary>
        /// Get the cloud host to use with your own choice of HTTP clients.
        /// </summary>
        /// <returns>The cloud host URL</returns>
		public static string GetCloudHost()
		{
			RequireAppReady ();
			return cloudProps.GetCloudHost ();
		}

        /// <summary>
        /// Shortcut to get the FHAuthSession instance
        /// </summary>
        /// <returns></returns>
        public static FHAuthSession GetAuthSession()
        {
            return FHAuthSession.Instance;
        }

        /// <summary>
        /// If you decide to use own choice of HTTP client and want to use the built-in analytics function of FeedHenry cloud,
        /// you need to add the returnd object as part of the request body with the key "__fh".
        /// </summary>
        /// <returns>The default request parameters</returns>
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
            string sessionToken = FHAuthSession.Instance.GetToken();
            if (null != sessionToken)
            {
                defaults["sessionToken"] = sessionToken;
            }
			return defaults;
		}

        /// <summary>
        /// If you decide to use own choice of HTTP client and want to use the built-in analytics function of FeedHenry cloud,
        /// you need to add the returned object as HTTP headers to each cloud request.
        /// </summary>
        /// <returns>The default HTTP request headers</returns>
		public static IDictionary<string, string> GetDefaultParamsAsHeaders()
		{
			IDictionary<string, object> defaultParams = GetDefaultParams ();
			IDictionary<string, string> headers = new Dictionary<string, string> ();
			foreach (var item in defaultParams) {
				string headername = "X-FH-" + item.Key;
                if (null != item.Value)
                {
                    headers.Add(headername, JsonConvert.SerializeObject(item.Value));
                }
				
			}
            if (null != FHConfig.getInstance().GetAppKey())
            {
                headers.Add("X-FH-AUTH-APP", FHConfig.getInstance().GetAppKey());
            }
            
			return headers;
		}
        /// <summary>
        /// If you want to receive push notifications call this method with a event handler that will receive the notifications
        /// </summary>
        /// <param name="HandleNotification">The andlerl that will receive the notifications</param>
        public static async void RegisterPush(EventHandler<PushReceivedEvent> HandleNotification)
        {
            await ServiceFinder.Resolve<IPush>().Register(HandleNotification);
        }

        /// <summary>
        /// Update the categories used for push notifications
        /// </summary>
        /// <param name="categories">then new categories</param>
        public static async void SetPushCategories(List<string> categories)
        {
            await ServiceFinder.Resolve<IPush>().SetCategories(categories);
        }

        /// <summary>
        /// Update the alias used for the push notifications
        /// </summary>
        /// <param name="alias">the alias for this device</param>
        public static async void SetPushAlias(string alias)
        {
            await ServiceFinder.Resolve<IPush>().SetAlias(alias);
        }

        /// <summary>
        /// Set the log levels. 
        /// VERBOSE=1
        /// DEBUG=2
        /// INFO=3
        /// WARNING=4
        /// ERROR=5
        /// NONE=Int16.MaxValue
        /// </summary>
        /// <param name="level">One of the options above</param>
		public static void SetLogLevel(int level)
		{
			ILogService logService = ServiceFinder.Resolve<ILogService> ();
			logService.SetLogLevel (level);
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
