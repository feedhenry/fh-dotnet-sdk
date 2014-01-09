using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FHSDK.Services
{
    public interface INetworkService
    {
        Task<bool> IsOnlineAsync();
    }
}
