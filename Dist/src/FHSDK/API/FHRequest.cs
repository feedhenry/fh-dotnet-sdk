using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using FHSDK.Config;

namespace FHSDK.API
{
    /// <summary>
    ///     Base class for all the API requests
    /// </summary>
    public abstract class FHRequest
    {
        private const double DefaultTimeout = 30*1000;
        protected readonly FHConfig AppConfig = FHConfig.GetInstance();
        private IDictionary<string, string> _requestHeaders;
        private string _requestMethod = "POST";

        /// <summary>
        ///     The app configurations
        /// </summary>
        private TimeSpan _timeout = TimeSpan.FromMilliseconds(DefaultTimeout);

        /// <summary>
        ///     Get or set the timeout value
        /// </summary>
        public TimeSpan TimeOut
        {
            private get { return _timeout; }

            set { _timeout = value; }
        }

        /// <summary>
        ///     Get or set the http request method
        /// </summary>
        public string RequestMethod
        {
            private get { return _requestMethod; }

            set { _requestMethod = value; }
        }

        /// <summary>
        ///     Get or set the http request headers
        /// </summary>
        public IDictionary<string, string> RequestHeaders
        {
            private get
            {
                var defaultHeaders = FH.GetDefaultParamsAsHeaders();
                if (null == _requestHeaders) return defaultHeaders;
                foreach (var item in _requestHeaders)
                {
                    var key = item.Key;
                    if (!defaultHeaders.ContainsKey(key))
                    {
                        defaultHeaders.Add(key, item.Value);
                    }
                }
                return defaultHeaders;
            }

            set { _requestHeaders = value; }
        }

        /// <summary>
        ///     Execute the request asynchronously
        /// </summary>
        /// <returns>Server response</returns>
        public virtual async Task<FHResponse> ExecAsync()
        {
            var uri = GetUri();
            var requestParams = GetRequestParams();
            var fhres =
                await FHHttpClient.FHHttpClient.SendAsync(uri, RequestMethod, RequestHeaders, requestParams, TimeOut);
            return fhres;
        }

        /// <summary>
        ///     Get the default request parameters
        /// </summary>
        /// <returns></returns>
        protected IDictionary<string, object> GetDefaultParams()
        {
            return FH.GetDefaultParams();
        }

        /// <summary>
        ///     Construct the remote uri based on the request type
        /// </summary>
        /// <returns></returns>
        protected abstract Uri GetUri();

        /// <summary>
        ///     Construct the request data based on the request type
        /// </summary>
        /// <returns></returns>
        protected abstract object GetRequestParams();
    }
}