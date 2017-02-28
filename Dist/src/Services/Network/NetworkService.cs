using System;
using System.Threading.Tasks;
using FHSDK.Services.Network;

namespace FHSDK.Services
{
    /// <summary>
    /// Network service provider for iOS
    /// </summary>
	public class NetworkService: INetworkService
	{
		private bool IsConnected{ get; set;}

		public NetworkService ()
		{
			CheckNetworkStatus ();
			Reachability.ReachabilityChanged += (object sender, EventArgs e) => 
			{
				CheckNetworkStatus();
			};
		}

		public async Task<bool> IsOnlineAsync()
		{
			return await Task.Run( () => {
				return IsConnected;
			});
		}

		public bool IsOnline ()
		{
			return IsConnected;
		}

		private void CheckNetworkStatus()
		{
			var remoteHostStatus = Reachability.RemoteHostStatus();
			var internetStatus = Reachability.InternetConnectionStatus();
			var localWifiStatus = Reachability.LocalWifiConnectionStatus();
			IsConnected = (internetStatus == NetworkStatus.ReachableViaCarrierDataNetwork ||
				internetStatus == NetworkStatus.ReachableViaWiFiNetwork) ||
				(localWifiStatus == NetworkStatus.ReachableViaCarrierDataNetwork ||
					localWifiStatus == NetworkStatus.ReachableViaWiFiNetwork) ||
				(remoteHostStatus == NetworkStatus.ReachableViaCarrierDataNetwork ||
					remoteHostStatus == NetworkStatus.ReachableViaWiFiNetwork);
		}
	}
}

