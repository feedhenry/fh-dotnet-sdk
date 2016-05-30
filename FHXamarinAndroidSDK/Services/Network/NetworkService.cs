using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Net;
using FHSDK.Services.Network;

namespace FHSDK.Services
{
    /// <summary>
    ///     Network service provider for Android
    /// </summary>
    public class NetworkService : INetworkService
    {
        public async Task<bool> IsOnlineAsync()
        {
            return await Task.Run(() => CheckNetworkStatus()).ConfigureAwait(false);
        }

        public bool IsOnline()
        {
            return CheckNetworkStatus();
        }

        private bool CheckNetworkStatus()
        {
            var connMgr = (ConnectivityManager) Application.Context.GetSystemService(Context.ConnectivityService);
            var networkInfo = connMgr.ActiveNetworkInfo;
            return null != networkInfo && networkInfo.IsConnected;
        }
    }
}