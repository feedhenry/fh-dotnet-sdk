using System;

namespace FHSDK.API
{
    /// <summary>
    ///     Class represents init requests
    /// </summary>
    public class FHInitRequest : FHRequest
    {
        private const string InitPath = "box/srv/1.1/app/init";

        /// <summary>
        ///     Construct the remote uri based on the request type
        /// </summary>
        /// <returns></returns>
        protected override Uri GetUri()
        {
            return new Uri(string.Format("{0}/{1}", AppConfig.GetHost(), InitPath));
        }

        /// <summary>
        ///     Construct the request data based on the request type
        /// </summary>
        /// <returns></returns>
        protected override object GetRequestParams()
        {
            return GetDefaultParams();
        }
    }
}