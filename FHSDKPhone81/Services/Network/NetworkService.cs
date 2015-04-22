using System;
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

        public async Task<bool> IsOnlineAsync()
        {
            return await Task.Run<bool>(() => { return IsOnline(); });
        }

        public bool IsOnline()
        {
            return NetworkInterface.GetIsNetworkAvailable();
        }
    }
}
