using Microsoft.Phone.Info;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FHSDK.FHHttpClient
{
    public abstract class FHRequest
    {
        const double DEFAULT_TIMEOUT = 30*1000;
        protected AppProps appProps;
        private TimeSpan timeout = TimeSpan.FromMilliseconds(DEFAULT_TIMEOUT);

        public FHRequest(AppProps appProps)
        {
            this.appProps = appProps;
        }

        public TimeSpan TimeOut
        {
            get
            {
                return timeout;
            }

            set
            {
                this.timeout = value;
            }
        }

        public async Task<FHResponse> execAsync()
        {
            string uri = GetUri();
            IDictionary<string, object> requestParams = GetRequestParams();
            FHResponse fhres = await FHHttpClient.PostAsync(uri, requestParams, this.timeout);
            return fhres;
        }

        protected IDictionary<string, object> GetDefaultParams()
        {
            Dictionary<string, object> defaults = new Dictionary<string, object>();
            defaults["appid"] = appProps.appid;
            defaults["appkey"] = appProps.appkey;
            defaults["cuid"] = this.UUID;
            defaults["destination"] = "windowsphone";
            defaults["sdk_version"] = "FH_WINDOWNSPHONE_SDK/" + FH.SDK_VERSION;
            if (null != this.appProps.projectid)
            {
                defaults["projectid"] = this.appProps.projectid;
            }
            JObject initInfo = GetInitInfo();
            if (null != initInfo)
            {
                defaults["init"] = initInfo;
            }
            return defaults;
        }

        protected JObject GetInitInfo()
        {
            IsolatedStorageSettings settings = IsolatedStorageSettings.ApplicationSettings;
            string initValue;
            settings.TryGetValue<string>("init", out initValue);
            JObject initInfo = null;
            if (null != initValue)
            {
                initInfo = JObject.Parse(initValue);
            }
            return initInfo;
        }

        protected string UUID
        {
            get
            {
                string retVal = "";
                object uuid;
                UserExtendedProperties.TryGetValue("ANID2", out uuid);
                if (null != uuid)
                {
                    retVal = uuid.ToString().Substring(2, 32);
                }
                else
                {
                    DeviceExtendedProperties.TryGetValue("DeviceUniqueId", out uuid);
                    if (null != uuid)
                    {
                        retVal = Convert.ToBase64String((byte[])uuid);
                    }
                }
                return retVal;
            }
        }

        public abstract string GetUri();
        public abstract IDictionary<string, object> GetRequestParams();

    }

    public class FHInitRequest : FHRequest
    {
        const string INIT_PATH = "app/init";
        
        public FHInitRequest(AppProps appProps)
            : base(appProps)
        {
        }

        public override string GetUri()
        {
            return String.Format("{0}/{1}", this.appProps.host, INIT_PATH);
        }

        public override IDictionary<string, object> GetRequestParams()
        {
            return GetDefaultParams();
        }
    }

    public class FHActRequest : FHRequest
    {
        private JObject cloudProps;

        public string RemoteAct { get; set; }
        public IDictionary<string, object> RequestParams { get; set; }

        public FHActRequest(AppProps appProps, JObject cloudProps)
            : base(appProps)
        {
            this.cloudProps = cloudProps;
        }

        public async Task<FHResponse> execAsync(string remoteAct, IDictionary<string, object> requestParams)
        {
            this.RemoteAct = remoteAct;
            this.RequestParams = requestParams;
            return await this.execAsync();
        }

        public override string GetUri()
        {
            if (null == this.RemoteAct)
            {
                throw new InvalidOperationException("RemoteAction can not be null");
            }
            string uri = null;
            string appMode = this.appProps.mode;

            if (null != cloudProps["url"])
            {
                uri = (string) cloudProps["url"];
            }
            else
            {
                JObject hosts = (JObject) cloudProps["hosts"];
                if ("dev" == appMode)
                {
                    uri = (string) hosts["debugCloudUrl"];
                }
                else
                {
                    uri = (string) hosts["releaseCloudUrl"];
                }
            }
            uri = uri.EndsWith("/") ? uri.Substring(0, uri.Length - 1) : uri;
            return String.Format("{0}/{1}/{2}", uri, "cloud", this.RemoteAct);
        }

        public override IDictionary<string, object> GetRequestParams()
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            if(null != this.RequestParams)
            {
                data = new Dictionary<string, object>(this.RequestParams);
            }
            IDictionary<string, object> defaultParams = GetDefaultParams();
            data["__fh"] = defaultParams;
            return data;
        }
    }

}
