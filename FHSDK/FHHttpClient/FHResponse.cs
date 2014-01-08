using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace FHSDK.FHHttpClient
{
    public class FHResponse
    {
        private HttpStatusCode statusCode;
        private string rawResponse;
        private FHException error;

        public FHResponse(HttpStatusCode statusCode, string rawResponse)
        {
            this.statusCode = statusCode;
            this.rawResponse = rawResponse;
            this.error = null;
        }

        public FHResponse(HttpStatusCode statusCode, string rawResponse, FHException error)
        {
            this.statusCode = statusCode;
            this.rawResponse = rawResponse;
            this.error = error;
        }

        public FHResponse(string rawResponse, FHException error)
        {
            this.rawResponse = rawResponse;
            this.error = error;
        }

        public FHResponse(FHException error)
        {
            this.rawResponse = null;
            this.statusCode = 0;
            this.error = error;
        }

        public HttpStatusCode StatusCode
        {
            get
            {
                return statusCode;
            }
        }

        public string RawResponse
        {
            get
            {
                return rawResponse;
            }
        }

        public FHException Error
        {
            get
            {
                return error;
            }
        }

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
    }
}
