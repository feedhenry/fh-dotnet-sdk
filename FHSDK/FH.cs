using FHSDK.FHHttpClient;
using FHSDK.Services;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace FHSDK
{
    /// <summary>
    /// This is the main FeedHenry SDK class
    /// </summary>
    public class FH
    {
        const string APP_PROP_FILE = "fh.config";
        const double DEFAULT_TIMEOUT = 30 * 1000;
        private static AppProps appProps;
        private static bool appReady = false;
        private static JObject cloudProps = null;
        private static TimeSpan timeout = TimeSpan.FromMilliseconds(DEFAULT_TIMEOUT);


        /// <summary>
        /// Get the current version of the FeedHenry WindowsPhone SDk
        /// </summary>
        public static string SDK_VERSION
        {
            get
            {
                Assembly assembly = Assembly.GetExecutingAssembly();
                string name = assembly.FullName;
                AssemblyName asm = new AssemblyName(name);
                return asm.Version.ToString();
            }
        }

        /// <summary>
        /// Get the app configurations defined in fh.config file
        /// </summary>
        /// <returns>The app configurations defined in fh.config file</returns>
        public static async Task<AppProps> GetAppProps()
        {
            if (null == appProps)
            {
                appProps = await ReadAppPropsAsync();
            }
            return appProps;
        }

        /// <summary>
        /// Get or Set the timeout value for all the requests. Default is 30 seconds.
        /// </summary>
        public static TimeSpan TimeOut
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
        public static async Task<bool> Init()
        {
            if (null == appProps)
            {
                appProps = await ReadAppPropsAsync();
            }
            if (!appReady)
            {
                FHInitRequest initRequest = new FHInitRequest(appProps);
                initRequest.TimeOut = timeout;
                FHResponse initRes = await initRequest.execAsync();
                if (null == initRes.Error)
                {
                    cloudProps = initRes.GetResponseAsJObject();
                    appReady = true;
                    JToken initValue = cloudProps["init"];
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
            FHActRequest actRequest = new FHActRequest(appProps, cloudProps);
            actRequest.TimeOut = timeout;
            return await actRequest.execAsync(remoteAct, actParams);
        }

        public static async Task<FHResponse> Auth(string policyId)
        {
            RequireAppReady();
            FHAuthRequest authRequest = new FHAuthRequest(appProps);
            authRequest.TimeOut = timeout;
            authRequest.SetAuthPolicyId(policyId);
            return await authRequest.execAsync();
        }

        public static async Task<FHResponse> Auth(string policyId, string userName, string userPassword)
        {
            RequireAppReady();
            FHAuthRequest authRequest = new FHAuthRequest(appProps);
            authRequest.TimeOut = timeout;
            authRequest.SetAuthUser(policyId, userName, userPassword);
            return await authRequest.execAsync();
        }

        private static void RequireAppReady()
        {
            if (!appReady)
            {
                throw new InvalidOperationException("FH is not ready. Have you called FH.Init?");
            }
        }

        /// <summary>
        /// Read the app configurations from fh.config file
        /// </summary>
        /// <returns></returns>
        private static async Task<AppProps> ReadAppPropsAsync()
        {
            IDeviceService deviceService = (IDeviceService) ServiceFinder.Resolve<IDeviceService>();
            string appPropsStr = await deviceService.ReadResourceAsString(APP_PROP_FILE);
            if (appPropsStr != null)
            {
                Debug.WriteLine(String.Format("appProps = {0}", appPropsStr));
                AppProps props = JsonConvert.DeserializeObject<AppProps>(appPropsStr);
                return props;
            }
            else
            {
                throw new Exception(APP_PROP_FILE + " not found");
            }
        }

        /// <summary>
        /// Save app init info. Mainly used for analytics.
        /// </summary>
        /// <param name="initInfo"></param>
        private static void SaveInitInfo(string initInfo)
        {
            IDeviceService deviceService = (IDeviceService)ServiceFinder.Resolve<IDeviceService>();
            deviceService.SaveData("init", initInfo);
        }

    }

    /// <summary>
    /// Describe the app configuration options in fh.config file.
    /// </summary>
    public class AppProps
    {
        /// <summary>
        /// Get or Set the app host
        /// </summary>
        public string host { get; set; }
        /// <summary>
        /// Get or set the project id
        /// </summary>
        public string projectid { get; set; }
        /// <summary>
        /// Get or set the appid
        /// </summary>
        public string appid { get; set; }
        /// <summary>
        /// Get or set the app API key
        /// </summary>
        public string appkey { get; set; }
        /// <summary>
        /// Get or set the app mode
        /// </summary>
        public string mode { get; set; }
    }

}
