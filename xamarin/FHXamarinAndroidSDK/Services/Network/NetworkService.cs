using System;
using Android.Net;
using Android.App;
using Android.Content;
using System.Threading.Tasks;
using FHSDK.Services.Network;

namespace FHSDK.Services
{
    /// <summary>
    /// Network service provider for Android
    /// </summary>
	public class NetworkService: INetworkService
	{
		public async Task<bool> IsOnlineAsync()
		{
			return await Task.Run(() => checkNetworkStatus()).ConfigureAwait(false);
		}

		public bool IsOnline ()
		{
			return checkNetworkStatus ();
		}

		private bool checkNetworkStatus()
		{
			ConnectivityManager connMgr = (ConnectivityManager) Application.Context.GetSystemService(Context.ConnectivityService);
			NetworkInfo networkInfo = connMgr.ActiveNetworkInfo;
			if (null != networkInfo && networkInfo.IsConnected) {
				return true;
			} else {
				return false;
			}
		}

	}
}

