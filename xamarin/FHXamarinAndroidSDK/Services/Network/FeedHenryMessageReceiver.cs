using Android.App;
using Android.Content;
using Android.OS;
using Android.Gms.Gcm;
using Android.Util;
using FHSDK.Services.Network;
using System.Collections.Generic;
using System.Linq;

namespace FHSDK.Services
{
	[Service (Exported = false), IntentFilter (new [] { "com.google.android.c2dm.intent.RECEIVE" })]
	public class FeedHenryMessageReceiver : GcmListenerService
	{
		public override void OnMessageReceived (string from, Bundle data)
		{
			
			var push = ServiceFinder.Resolve<IPush>() as Push;
			string message = data.GetString ("alert");
			Dictionary<string, string> messageData = new Dictionary<string,string> ();


			foreach (string key in data.KeySet()) {
				messageData.Add(key, data.GetString(key));	
			}

			(push.Registration as GcmRegistration).OnPushNotification (message, messageData);
		}
	}
}

