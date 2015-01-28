using FHSDK.Services;
using System.Net.NetworkInformation;
using System.Threading.Tasks;

namespace FHSDK81.Services
{
    /// <summary>
    /// Network service provider for windows phone
    /// </summary>
    class NetworkService : INetworkService
    {
        public NetworkService()
        {
        }

        public async Task<bool> IsOnlineAsync()
        {
            return await Task.Run(() =>
            {
                return IsOnline();
            });
        }

        public bool IsOnline()
        {
            return NetworkInterface.GetIsNetworkAvailable();
        }
    }
}
