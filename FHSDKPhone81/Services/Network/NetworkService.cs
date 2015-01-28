using System.Net.NetworkInformation;
using System.Threading.Tasks;

namespace FHSDK.Services
{
    /// <summary>
    /// Network service provider for windows phone
    /// </summary>
    class NetworkService : INetworkService
    {
        public NetworkService()
        {
        }

        public Task<bool> IsOnlineAsync()
        {
            return Task.Factory.StartNew(() => IsOnline());
        }

        public bool IsOnline()
        {
            return NetworkInterface.GetIsNetworkAvailable();
        }
    }
}
