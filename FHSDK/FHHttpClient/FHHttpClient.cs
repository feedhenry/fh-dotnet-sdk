using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net.Http;
using System.Text;

namespace FHSDK.FHHttpClient
{
    /// <summary>
    /// HttpClient used by the SDK
    /// </summary>
    public class FHHttpClient
    {

        const int BUFFER_SIZE = 10*1024;

        /// <summary>
        /// Check if the device is online
        /// </summary>
        /// <returns></returns>
        private static async Task<bool> IsOnlineAsync()
        {
            return await Task.Run(() =>
            {
                return (Microsoft.Phone.Net.NetworkInformation.NetworkInterface.NetworkInterfaceType != Microsoft.Phone.Net.NetworkInformation.NetworkInterfaceType.None);
            });
        }

        /// <summary>
        /// Post request to the remote uri
        /// </summary>
        /// <param name="uri">The remote uri</param>
        /// <param name="requestData">The request data</param>
        /// <param name="timeout">Timeout in milliseconds</param>
        /// <returns>Server response</returns>
        public static async Task<FHResponse> PostAsync(string uri, IDictionary<string, object> requestData, TimeSpan timeout)
        {
            bool online = await IsOnlineAsync();
            FHResponse fhres = null;
            if (!online)
            {
                FHException exception = new FHException("offline", FHException.ErrorCode.NetworkError);
                fhres = new FHResponse(exception);
                return fhres;
            }
            HttpClient httpClient = new HttpClient();
            try
            {
                System.Uri requestUri = new Uri(uri);
                httpClient.DefaultRequestHeaders.Add("User-Agent", "FHSDK/WindowsPhone");
                httpClient.MaxResponseContentBufferSize = BUFFER_SIZE;
                httpClient.Timeout = timeout;

                HttpContent requestContent = new StringContent("", Encoding.UTF8, "application/json");
                if (null != requestData)
                {
                    string requestDataStr = JsonConvert.SerializeObject(requestData);
                    if (null != requestDataStr)
                    {
                        requestContent = new StringContent(requestDataStr, Encoding.UTF8, "application/json");
                    }
                }

                HttpResponseMessage responseMessage = await httpClient.PostAsync(requestUri, requestContent);
                string responseStr = await responseMessage.Content.ReadAsStringAsync();
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
                FHException fhexception = new FHException("HttpError", FHException.ErrorCode.HttpError, he);
                fhres = new FHResponse(fhexception);
            }
            catch (Exception e)
            {
                FHException fhexception = new FHException("UnknownError", FHException.ErrorCode.UnknownError, e);
                fhres = new FHResponse(fhexception);
            }
            httpClient.Dispose();
            return fhres;
        }
    }
}
