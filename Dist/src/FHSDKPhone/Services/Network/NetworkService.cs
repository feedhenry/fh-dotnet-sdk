using System.Threading.Tasks;
using FHSDK.Services.Network;
using Microsoft.Phone.Net.NetworkInformation;

namespace FHSDK.Services
{
    /// <summary>
    ///     Network service provider for windows phone
    /// </summary>
    internal class NetworkService : INetworkService
    {
        public async Task<bool> IsOnlineAsync()
        {
            return await Task.Run(() => (NetworkInterface.NetworkInterfaceType != NetworkInterfaceType.None));
        }

        public bool IsOnline()
        {
            return (NetworkInterface.NetworkInterfaceType != NetworkInterfaceType.None);
        }
    }
}