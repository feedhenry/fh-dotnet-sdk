using System;
using FHSDK.API;
using System.Collections.Generic;

namespace FHSDK
{
    /// <summary>
    /// Class represents cloud requests.
    /// </summary>
	public class FHCloudRequest: FHRequest
	{
		private CloudProps cloudProps;

		private object requestParams;
		private string path = "";

		/// <summary>
		/// Get or set the request parameters
		/// </summary>
		public object RequestParams 
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

        /// <summary>
        /// Get or set the path of the cloud request
        /// </summary>
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

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="props">The cloud host info</param>
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
		protected override object GetRequestParams()
		{
			return this.requestParams;
		}


	}
}

