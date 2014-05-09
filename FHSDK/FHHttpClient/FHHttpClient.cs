using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text;
using FHSDK.Services;
using System.Diagnostics;
using System.Net.Http;
using System.Net;
using System.Diagnostics.Contracts;

namespace FHSDK.FHHttpClient
{
    /// <summary>
    /// Contains implementation of a HttpClient used by the FeedHenry .Net SDK. Defined in the FHSDK.dll assembly.
    /// </summary>
    [System.Runtime.CompilerServices.CompilerGenerated]
    class NamespaceDoc
    {

    }

    /// <summary>
    /// HttpClient used by the SDK
    /// </summary>
    public class FHHttpClient
    {

        const int BUFFER_SIZE = 10*1024;
		const string LOG_TAG = "FHHttpClient";

        /// <summary>
        /// Check if the device is online
        /// </summary>
        /// <returns></returns>
        private static async Task<bool> IsOnlineAsync()
        {
            INetworkService networkServiceProvider = (INetworkService) ServiceFinder.Resolve<INetworkService>();
            return await networkServiceProvider.IsOnlineAsync();
        }

		private static Uri BuildUri(Uri uri, string requestMethod, IDictionary<string, object> requestData)
		{
			if (!"POST".Equals (requestMethod.ToUpper ()) && !"PUT".Equals (requestMethod.ToUpper ())) {
				if (null != requestData) {
					UriBuilder ub = new UriBuilder (uri);
					List<string> qs = new List<string> ();
					foreach (var item in requestData) {
						qs.Add (String.Format ("{0}={1}", item.Key, JsonConvert.SerializeObject (item.Value)));
					}
					string query = String.Join (",", qs.ToArray ());
					string existingQuery = ub.Query;
					if (null != existingQuery && existingQuery.Length > 1) {
						ub.Query = existingQuery.Substring (1) + "&" + query;
					} else {
						ub.Query = query;
					}
					return ub.Uri;
				}
				return uri;
			}
			return uri;
		}

        /// <summary>
        /// Send request to the remote uri
        /// </summary>
        /// <param name="uri">The remote uri</param>
        /// <param name="requestMethod">The http request method</param>
        /// <param name="headers">The http reqeust headers</param>
        /// <param name="requestData">The request data</param>
        /// <param name="timeout">Timeout in milliseconds</param>
        /// <returns>Server response</returns>
		public static async Task<FHResponse> SendAsync(Uri uri, string requestMethod, IDictionary<string, string> headers, IDictionary<string, object> requestData, TimeSpan timeout)
        {
			Stopwatch timer = new Stopwatch ();

			ILogService logger = ServiceFinder.Resolve<ILogService> ();
            bool online = await IsOnlineAsync();
            FHResponse fhres = null;
            if (!online)
            {
                FHException exception = new FHException("offline", FHException.ErrorCode.NetworkError);
                fhres = new FHResponse(exception);
                return fhres;
            }
			Contract.Assert (null != uri, "No request uri defined");
			Contract.Assert (null != requestMethod, "No http request method defined");
			HttpClient httpClient = FHHttpClientFactory.Get ();

            try
            {
				logger.d(LOG_TAG, "Send request to " + uri, null);
				httpClient.DefaultRequestHeaders.Add("User-Agent", "FHSDK/DOTNET");
                httpClient.MaxResponseContentBufferSize = BUFFER_SIZE;
				httpClient.Timeout = timeout;
                

				HttpRequestMessage requestMessage = new HttpRequestMessage(new HttpMethod(requestMethod), BuildUri(uri, requestMethod, requestData));
				if(null != headers){
					foreach (var item in headers) {
						requestMessage.Headers.Add(item.Key, item.Value);
					}
				}

				if("POST".Equals(requestMethod.ToUpper()) || "PUT".Equals(requestMethod.ToUpper())){
					if(null != requestData){
						string requestDataStr = JsonConvert.SerializeObject(requestData);
						requestMessage.Content = new StringContent(requestDataStr, Encoding.UTF8, "application/json");
					}
				}

				timer.Start ();
				HttpResponseMessage responseMessage = await httpClient.SendAsync(requestMessage);
				timer.Stop();
				logger.d(LOG_TAG, "Reqeust Time: " + timer.ElapsedMilliseconds + "ms", null);
                string responseStr = await responseMessage.Content.ReadAsStringAsync();
				logger.d(LOG_TAG, "Response string is " + responseStr, null);
                if (responseMessage.IsSuccessStatusCode)
                {
                    fhres = new FHResponse(responseMessage.StatusCode, responseStr);
                }
                else
                {
                    FHException ex = new FHException("ServerError", FHException.ErrorCode.ServerError);
                    fhres = new FHResponse(responseMessage.StatusCode, responseStr, ex);
                }
            }
            catch (HttpRequestException he)
            {
				logger.e (LOG_TAG, "HttpRequestException", he);
                FHException fhexception = new FHException("HttpError", FHException.ErrorCode.HttpError, he);
                fhres = new FHResponse(fhexception);
            }
            catch (Exception e)
            {
				logger.e (LOG_TAG, "Exception", e);
                FHException fhexception = new FHException("UnknownError", FHException.ErrorCode.UnknownError, e);
                fhres = new FHResponse(fhexception);
            }
            httpClient.Dispose();
            return fhres;
        }
    }
}
