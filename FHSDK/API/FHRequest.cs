using FHSDK.Services;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using FHSDK.FHHttpClient;
using System.Net.Http;

namespace FHSDK.API
{
    /// <summary>
    /// Contains implementations for accessing FeedHenry APIs. Defined in the FHSDK.dll.
    /// </summary>
    [System.Runtime.CompilerServices.CompilerGenerated]
    class NamespaceDoc
    {

    }

    /// <summary>
    /// Base class for all the API requests
    /// </summary>
    public abstract class FHRequest
    {
        const double DEFAULT_TIMEOUT = 30*1000;
		protected FHConfig appConfig = FHConfig.getInstance ();


        /// <summary>
        /// The app configurations
        /// </summary>
        private TimeSpan timeout = TimeSpan.FromMilliseconds(DEFAULT_TIMEOUT);

		private string requestMethod = "POST";
		private IDictionary<string, string> requestHeaders = null;

        /// <summary>
        /// Constructor
        /// </summary>
        public FHRequest()
        {
        }

        /// <summary>
        /// Get or set the timeout value
        /// </summary>
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

        /// <summary>
        /// Get or set the http request method
        /// </summary>
		public string RequestMethod
		{
			get 
			{
				return requestMethod;
			}

			set
			{ 
				this.requestMethod = value;
			}

		}

        /// <summary>
        /// Get or set the http request headers
        /// </summary>
		public IDictionary<string, string> RequestHeaders
		{
			get
			{
				IDictionary<string, string> defaultHeaders = FH.GetDefaultParamsAsHeaders ();
				if (null != this.requestHeaders) {
					defaultHeaders.Concat (this.requestHeaders);
				}
				return defaultHeaders;
			}

			set
			{
				this.requestHeaders = value;
			}

		}



        /// <summary>
        /// Execute the request asynchronously
        /// </summary>
        /// <returns>Server response</returns>
        public virtual async Task<FHResponse> execAsync()
        {
			Uri uri = GetUri();
            object requestParams = GetRequestParams();
			FHResponse fhres = await FHHttpClient.FHHttpClient.SendAsync(uri, RequestMethod, RequestHeaders, requestParams, TimeOut);
            return fhres;
        }

        /// <summary>
        /// Get the default request parameters
        /// </summary>
        /// <returns></returns>
		protected IDictionary<string, object> GetDefaultParams()
        {
			return FH.GetDefaultParams ();
        }

        
			

        /// <summary>
        /// Construct the remote uri based on the request type
        /// </summary>
        /// <returns></returns>
		protected abstract Uri GetUri();

        /// <summary>
        /// Construct the request data based on the request type
        /// </summary>
        /// <returns></returns>
		protected abstract object GetRequestParams();

    }

    

    

    

}
