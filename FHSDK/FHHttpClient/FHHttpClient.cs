using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FHSDK.Services;
using FHSDK.Services.Log;
using FHSDK.Services.Network;
using Microsoft.Practices.ObjectBuilder2;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace FHSDK.FHHttpClient
{
    /// <summary>
    ///     Contains implementation of a HttpClient used by the FeedHenry .Net SDK. Defined in the FHSDK.dll assembly.
    /// </summary>
    [CompilerGenerated]
    internal class NamespaceDoc
    {
    }

    /// <summary>
    ///     HttpClient used by the SDK
    /// </summary>
    public class FHHttpClient
    {
        private const int BufferSize = 1024*1024;
        private const string LogTag = "FHHttpClient";

        /// <summary>
        ///     Check if the device is online
        /// </summary>
        /// <returns></returns>
        private static async Task<bool> IsOnlineAsync()
        {
            var networkServiceProvider = ServiceFinder.Resolve<INetworkService>();
            return await networkServiceProvider.IsOnlineAsync();
        }

        private static Uri BuildUri(Uri uri, string requestMethod, object requestData)
        {
            if ("POST".Equals(requestMethod.ToUpper()) || "PUT".Equals(requestMethod.ToUpper())) return uri;
            if (null == requestData) return uri;
            var ub = new UriBuilder(uri);
            var jToken = JToken.FromObject(requestData);
            var qs = ConvertToParam(null, jToken);

            var query = string.Join("&", qs.ToArray());
            var existingQuery = ub.Query;
            if (null != existingQuery && existingQuery.Length > 1)
            {
                ub.Query = existingQuery.Substring(1) + "&" + query;
            }
            else
            {
                ub.Query = query;
            }
            return ub.Uri;
        }

        private static List<string> ConvertToParam(string prefix, JToken jToken)
        {
            var qs = new List<string>();
            if (jToken.Type == JTokenType.Object)
            {
                var jObject = (JObject) jToken;
                foreach (var item in jObject)
                {
                    if (item.Value.Type == JTokenType.Object || item.Value.Type == JTokenType.Array)
                    {
                        ConvertToParam(item.Key, item.Value).ForEach(i => qs.Add(i));
                    }
                    else
                    {
                        var value = prefix != null ? $"{prefix}[{item.Key}]={item.Value}" : $"{item.Key}={item.Value}";
                        qs.Add(value);
                    }
                }
            }
            else if (jToken.Type == JTokenType.Array)
            {
                var jArray = (JArray) jToken;
                foreach (var item in jArray)
                {
                    if (item.Type == JTokenType.Array)
                    {
                        ConvertToParam(prefix + "[]", item).ForEach(p => qs.Add(p));
                    }
                    else
                    {
                        qs.Add($"{prefix}[]={item}");
                    }
                }
            }
            else
            {
                qs.Add(jToken.ToString());
            }
            return qs;
        }

        /// <summary>
        ///     Send request to the remote uri
        /// </summary>
        /// <param name="uri">The remote uri</param>
        /// <param name="requestMethod">The http request method</param>
        /// <param name="headers">The http reqeust headers</param>
        /// <param name="requestData">The request data</param>
        /// <param name="timeout">Timeout in milliseconds</param>
        /// <returns>Server response</returns>
        public static async Task<FHResponse> SendAsync(Uri uri, string requestMethod,
            IDictionary<string, string> headers, object requestData, TimeSpan timeout)
        {
            var timer = new Stopwatch();

            var logger = ServiceFinder.Resolve<ILogService>();
            var online = await IsOnlineAsync();
            FHResponse fhres;
            if (!online)
            {
                var exception = new FHException("offline", FHException.ErrorCode.NetworkError);
                fhres = new FHResponse(exception);
                return fhres;
            }
            Contract.Assert(null != uri, "No request uri defined");
            Contract.Assert(null != requestMethod, "No http request method defined");
            var httpClient = FHHttpClientFactory.Get();

            try
            {
                logger.d(LogTag, "Send request to " + uri, null);
                httpClient.DefaultRequestHeaders.Add("User-Agent", "FHSDK/DOTNET");
                httpClient.MaxResponseContentBufferSize = BufferSize;
                httpClient.Timeout = timeout;


                var requestMessage = new HttpRequestMessage(new HttpMethod(requestMethod),
                    BuildUri(uri, requestMethod, requestData));
                if (null != headers)
                {
                    foreach (var item in headers)
                    {
                        requestMessage.Headers.Add(item.Key, item.Value);
                    }
                }

                if (requestMethod != null && ("POST".Equals(requestMethod.ToUpper()) || "PUT".Equals(requestMethod.ToUpper())))
                {
                    if (null != requestData)
                    {
                        var requestDataStr = JsonConvert.SerializeObject(requestData);
                        requestMessage.Content = new StringContent(requestDataStr, Encoding.UTF8, "application/json");
                    }
                }

                timer.Start();
                var responseMessage = await httpClient.SendAsync(requestMessage, CancellationToken.None);
                timer.Stop();
                logger.d(LogTag, "Reqeust Time: " + timer.ElapsedMilliseconds + "ms", null);
                var responseStr = await responseMessage.Content.ReadAsStringAsync();
                logger.d(LogTag, "Response string is " + responseStr, null);
                if (responseMessage.IsSuccessStatusCode)
                {
                    fhres = new FHResponse(responseMessage.StatusCode, responseStr);
                }
                else
                {
                    var ex = new FHException("ServerError", FHException.ErrorCode.ServerError);
                    fhres = new FHResponse(responseMessage.StatusCode, responseStr, ex);
                }
            }
            catch (HttpRequestException he)
            {
                logger.e(LogTag, "HttpRequestException", he);
                var fhexception = new FHException("HttpError", FHException.ErrorCode.HttpError, he);
                fhres = new FHResponse(fhexception);
            }
            catch (Exception e)
            {
                logger.e(LogTag, "Exception", e);
                var fhexception = new FHException("UnknownError", FHException.ErrorCode.UnknownError, e);
                fhres = new FHResponse(fhexception);
            }
            httpClient.Dispose();
            return fhres;
        }
    }
}