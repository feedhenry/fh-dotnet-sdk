using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace tests.Mocks
{
    class MockHttpClient : HttpClient
    {
        public override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            Request = request;
            return Task.Factory.StartNew(() => new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent("ok") });
        }

        public HttpRequestMessage Request { get; private set; }
    }
}
