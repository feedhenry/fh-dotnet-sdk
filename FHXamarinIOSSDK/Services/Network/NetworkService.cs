using System;
using System.Threading.Tasks;

namespace FHSDK.Services
{
	public class NetworkService: INetworkService
	{
		private bool IsConnected{ get; set;}

		public NetworkService ()
		{
			Reachability.ReachabilityChanged += (object sender, EventArgs e) => 
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
			};
		}

		public async Task<bool> IsOnlineAsync()
		{
			return await Task.Run( () => {
				return IsConnected;
			});
		}
	}
}

