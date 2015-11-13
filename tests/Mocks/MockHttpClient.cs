using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace tests.Mocks
{
    public class MockHttpClient : HttpClient
    {
        public MockHttpClient()
        {
            Content = "ok";
        }

        public override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            Request = request;
            return Task.Factory.StartNew(() => new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(Content) });
        }

        public string Content { private get; set; }

        public HttpRequestMessage Request { get; private set; }
    }
}