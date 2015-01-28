using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
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
