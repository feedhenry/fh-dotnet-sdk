using System.Net.NetworkInformation;
using System.Threading.Tasks;

namespace FHSDK.Services.Network
{
    /// <summary>
    ///     Network service provider for windows phone
    /// </summary>
    internal class NetworkService : INetworkService
    {
        public async Task<bool> IsOnlineAsync()
        {
            return await Task.Run(() => IsOnline());
        }

        public bool IsOnline()
        {
            return NetworkInterface.GetIsNetworkAvailable();
        }
    }
}