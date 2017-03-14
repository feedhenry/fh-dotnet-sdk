using System;

namespace FHSDK.API
{
    /// <summary>
    ///     Class represents cloud requests.
    /// </summary>
    public class FHCloudRequest : FHRequest
    {
        private readonly CloudProps _cloudProps;

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="props">The cloud host info</param>
        public FHCloudRequest(CloudProps props)
        {
            RequestPath = "";
            _cloudProps = props;
        }

        /// <summary>
        ///     Get or set the request parameters
        /// </summary>
        public object RequestParams { private get; set; }

        /// <summary>
        ///     Get or set the path of the cloud request
        /// </summary>
        public string RequestPath { private get; set; }

        /// <summary>
        ///     Construct the remote uri based on the request type
        /// </summary>
        /// <returns></returns>
        protected override Uri GetUri()
        {
            var host = _cloudProps.GetCloudHost();
            var pathWithoutStartingSlash = RequestPath.StartsWith("/") ? RequestPath.Substring(1) : RequestPath;
            return new Uri(string.Format("{0}/{1}", host, pathWithoutStartingSlash));
        }

        /// <summary>
        ///     Construct the request data based on the request type
        /// </summary>
        /// <returns></returns>
        protected override object GetRequestParams()
        {
            return RequestParams;
        }
    }
}