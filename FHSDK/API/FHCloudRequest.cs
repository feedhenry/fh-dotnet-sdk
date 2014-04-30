using System;
using FHSDK.API;
using System.Collections.Generic;

namespace FHSDK
{
	public class FHCloudRequest: FHRequest
	{
		private CloudProps cloudProps;

		private IDictionary<string, object> requestParams;
		private string path = "";

		/// <summary>
		/// Get or set the request parameters
		/// </summary>
		public IDictionary<string, object> RequestParams 
		{ 
			get 
		    { 
				return requestParams;
		    }

			set 
			{
				this.requestParams = value;
			}
	    }

		public string RequestPath { 
			get 
			{
				return this.path;
			}
			set 
			{
				this.path = value;
			}
		}

		public FHCloudRequest (CloudProps props)
			: base()
		{
			this.cloudProps = props;
		}

		/// <summary>
		/// Construct the remote uri based on the request type
		/// </summary>
		/// <returns></returns>
		protected override Uri GetUri()
		{
			string host = this.cloudProps.GetCloudHost ();
			return new Uri(String.Format("{0}/{1}", host, this.path));
		}

		/// <summary>
		/// Construct the request data based on the request type
		/// </summary>
		/// <returns></returns>
		protected override IDictionary<string, object> GetRequestParams()
		{
			return this.requestParams;
		}


	}
}

