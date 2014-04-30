using System;
using Android.Net;
using Android.App;
using Android.Content;
using System.Threading.Tasks;

namespace FHSDK.Services
{
	public class NetworkService: INetworkService
	{
		public NetworkService ()
		{
		}

		public async Task<bool> IsOnlineAsync()
		{
			return await Task.Run( () => {
				return checkNetworkStatus();
			});
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

