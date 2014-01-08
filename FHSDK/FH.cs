using FHSDK.FHHttpClient;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.IsolatedStorage;
using System.Net;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Resources;
using System.Windows.Shapes;

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
        const string VERSION = "BUILD_VERSION";


        /// <summary>
        /// Get the current version of the FeedHenry WindowsPhone SDk
        /// </summary>
        public static string SDK_VERSION
        {
            get
            {
                return VERSION;
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
        /// IDictionary<string, object> dict = new Dictionary<string, object>();
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
            if (!appReady)
            {
                throw new InvalidOperationException("FH is not ready. Have you called FH.Init?");
            }
            FHActRequest actRequest = new FHActRequest(appProps, cloudProps);
            actRequest.TimeOut = timeout;
            return await actRequest.execAsync(remoteAct, actParams);
        }

        /// <summary>
        /// Read the app configurations from fh.config file
        /// </summary>
        /// <returns></returns>
        private static async Task<AppProps> ReadAppPropsAsync()
        {
            StreamResourceInfo streamInfo = Application.GetResourceStream(new Uri(APP_PROP_FILE, UriKind.Relative));
            if (streamInfo != null)
            {
                StreamReader sr = new StreamReader(streamInfo.Stream);
                string appPropsStr = await sr.ReadToEndAsync();
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
            IsolatedStorageSettings settings = IsolatedStorageSettings.ApplicationSettings;
            settings.Add("init", initInfo);
            settings.Save();
        }

    }

    /// <summary>
    /// Describe the app configuration options in fh.config file.
    /// </summary>
    public class AppProps
    {
        public string host { get; set; }
        public string projectid { get; set; }
        public string appid { get; set; }
        public string appkey { get; set; }
        public string mode { get; set; }
    }

}
