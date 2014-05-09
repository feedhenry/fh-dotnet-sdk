using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FHSDK.Services
{
    /// <summary>
    /// A service interface to provide the network information of the device
    /// </summary>
    public interface INetworkService
    {
        /// <summary>
        /// Check if the device is online
        /// </summary>
        /// <returns>if the device is online</returns>
        Task<bool> IsOnlineAsync();
    }
}
