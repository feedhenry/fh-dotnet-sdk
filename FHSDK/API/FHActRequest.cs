using System;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Diagnostics.Contracts;

namespace FHSDK.API
{
	/// <summary>
	/// Class represents act requests
	/// </summary>
	public class FHActRequest : FHRequest
	{
		private readonly CloudProps _cloudProps;

		/// <summary>
		/// Get or set the remote cloud function name
		/// </summary>
		public string RemoteAct { get; set; }

		/// <summary>
		/// Get or set the request parameters
		/// </summary>
		public object RequestParams { get; set; }

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="cloudProps"></param>
		public FHActRequest(CloudProps cloudProps)
		{
			_cloudProps = cloudProps;
		}

		/// <summary>
		/// Execute the act request asynchronously
		/// </summary>
		/// <param name="remoteAct">The name of the cloud action</param>
		/// <param name="requestParams">The request parameters</param>
		/// <returns></returns>
        public async Task<FHResponse> ExecAsync(string remoteAct, object requestParams)
		{
			RemoteAct = remoteAct;
			RequestParams = requestParams;
			return await ExecAsync();
		}

		/// <summary>
		/// Construct the remote uri based on the request type
		/// </summary>
		/// <returns></returns>
		protected override Uri GetUri()
		{
			Contract.Assert (null != RemoteAct, "remote act is not defined");
			var host = _cloudProps.GetCloudHost ();
			return new Uri(string.Format("{0}/{1}/{2}", host, "cloud", RemoteAct));
		}

		/// <summary>
		/// Construct the request data based on the request type
		/// </summary>
		/// <returns></returns>
		protected override object GetRequestParams()
		{
            JObject data = new JObject();
			if(null != RequestParams)
			{
                
                data = JObject.FromObject(RequestParams);
			}
			IDictionary<string, object> defaultParams = GetDefaultParams();
			data["__fh"] = JToken.FromObject(defaultParams);
			return data;
		}
	}
}

