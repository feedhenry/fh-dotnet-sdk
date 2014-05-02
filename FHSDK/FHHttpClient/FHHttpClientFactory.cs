using System;
using System.Net.Http;

namespace FHSDK.FHHttpClient
{
	public static class FHHttpClientFactory
	{
		public static Func<HttpClient> Get { get; set; }

		static FHHttpClientFactory()
		{
			Get = (() => new HttpClient());
		}
	}
}

