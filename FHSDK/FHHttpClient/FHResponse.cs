using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace FHSDK
{
    /// <summary>
    /// </summary>
    public class FHResponse
    {
        private HttpStatusCode statusCode;
        private string rawResponse;
        private FHException error;

        /// <summary>
        ///
        /// </summary>
        /// <param name="statusCode"></param>
        /// <param name="rawResponse"></param>
        public FHResponse(HttpStatusCode statusCode, string rawResponse)
        {
            this.statusCode = statusCode;
            this.rawResponse = rawResponse;
            this.error = null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="statusCode"></param>
        /// <param name="rawResponse"></param>
        /// <param name="error"></param>
        public FHResponse(HttpStatusCode statusCode, string rawResponse, FHException error)
        {
            this.statusCode = statusCode;
            this.rawResponse = rawResponse;
            this.error = error;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rawResponse"></param>
        /// <param name="error"></param>
        public FHResponse(string rawResponse, FHException error)
        {
            this.rawResponse = rawResponse;
            this.error = error;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="error"></param>
        public FHResponse(FHException error)
        {
            this.rawResponse = null;
            this.statusCode = 0;
            this.error = error;
        }

        /// <summary>
        /// Get the status code of the response
        /// </summary>
        public HttpStatusCode StatusCode
        {
            get
            {
                return statusCode;
            }
        }

        /// <summary>
        /// Get the raw response data
        /// </summary>
        public string RawResponse
        {
            get
            {
                return rawResponse;
            }
        }

        /// <summary>
        /// Get the response error. Should be null if the request is successful.
        /// </summary>
        public FHException Error
        {
            get
            {
                return error;
            }
        }

        /// <summary>
        /// Get the response data as JSON object
        /// </summary>
        /// <returns></returns>
        public JObject GetResponseAsJObject()
        {
            if (null != rawResponse)
            {
                return JObject.Parse(rawResponse);
            }
            else
            {
                return JObject.Parse(@"{}");
            }

        }

        /// <summary>
        /// Get the response data as JSON array
        /// </summary>
        /// <returns></returns>
        public JArray GetResponseAsJArray()
        {
            if (null != rawResponse)
            {
                return JArray.Parse(rawResponse);
            }
            else
            {
                return JArray.Parse(@"[]");
            }
        }

		public IDictionary<string, object> GetResponseAsDictionary()
		{
			Dictionary<string, object> dict = new Dictionary<string, object> ();
			if (null != rawResponse) {
				dict = JsonConvert.DeserializeObject<Dictionary<string, object>> (rawResponse);
			}
			return dict;
		}
    }
}
