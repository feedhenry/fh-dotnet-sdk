using System;
using System.Net.Http;

namespace FHSDK.FHHttpClient
{
    /// <summary>
    /// Create a new instance of HttpClient using the default implementation. 
    /// You can override this functin to return your own instance of HttpClient.
    /// </summary>
	public static class FHHttpClientFactory
	{
		public static Func<HttpClient> Get { get; set; }

		static FHHttpClientFactory()
		{
			Get = (() => new HttpClient());
		}
	}
}

