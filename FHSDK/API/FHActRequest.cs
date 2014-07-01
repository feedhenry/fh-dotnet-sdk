using System;
using FHSDK.FHHttpClient;
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
		private CloudProps cloudProps;

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
			: base()
		{
			this.cloudProps = cloudProps;
		}

		/// <summary>
		/// Execute the act request asynchronously
		/// </summary>
		/// <param name="remoteAct">The name of the cloud action</param>
		/// <param name="requestParams">The request parameters</param>
		/// <returns></returns>
        public async Task<FHResponse> execAsync(string remoteAct, object requestParams)
		{
			this.RemoteAct = remoteAct;
			this.RequestParams = requestParams;
			return await this.execAsync();
		}

		/// <summary>
		/// Construct the remote uri based on the request type
		/// </summary>
		/// <returns></returns>
		protected override Uri GetUri()
		{
			Contract.Assert (null != RemoteAct, "remote act is not defined");
			string host = this.cloudProps.GetCloudHost ();
			return new Uri(String.Format("{0}/{1}/{2}", host, "cloud", this.RemoteAct));
		}

		/// <summary>
		/// Construct the request data based on the request type
		/// </summary>
		/// <returns></returns>
		protected override object GetRequestParams()
		{
            JObject data = new JObject();
			if(null != this.RequestParams)
			{
                
                data = JObject.FromObject(this.RequestParams);
			}
			IDictionary<string, object> defaultParams = GetDefaultParams();
			data["__fh"] = JToken.FromObject(defaultParams);
			return data;
		}
	}
}

