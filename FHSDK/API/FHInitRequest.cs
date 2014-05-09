using System;
using FHSDK.FHHttpClient;
using System.Collections.Generic;

namespace FHSDK.API
{
	/// <summary>
	/// Class represents init requests
	/// </summary>
	public class FHInitRequest : FHRequest
	{
		const string INIT_PATH = "box/srv/1.1/app/init";

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="appProps"></param>
		public FHInitRequest()
			: base()
		{
		}

		/// <summary>
		/// Construct the remote uri based on the request type
		/// </summary>
		/// <returns></returns>
		protected override Uri GetUri()
		{
			return new Uri(String.Format("{0}/{1}", appConfig.GetHost(), INIT_PATH));
		}

		/// <summary>
		/// Construct the request data based on the request type
		/// </summary>
		/// <returns></returns>
		protected override IDictionary<string, object> GetRequestParams()
		{
			return GetDefaultParams();
		}
	}
}

