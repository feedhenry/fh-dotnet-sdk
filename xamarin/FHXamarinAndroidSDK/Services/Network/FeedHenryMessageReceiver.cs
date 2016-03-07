using Android.App;
using Android.Content;
using Android.OS;
using Android.Gms.Gcm;
using Android.Util;
using FHSDK.Services.Network;
using System.Collections.Generic;
using System.Linq;
using System;

namespace FHSDK.Services
{
	public abstract class FeedHenryMessageReceiver : GcmListenerService
	{

		private const string DEFAULT_MESSAGE_HANDLER_KEY = "DEFAULT_MESSAGE_HANDLER_KEY";

		public override async void OnMessageReceived (string from, Bundle data)
		{

			if (!ServiceFinder.IsRegistered<IPush> ()) {
					await FHClient.Init ();
					FH.RegisterPush(DefaultHandleEvent);
			}

			var push = ServiceFinder.Resolve<IPush>() as Push;
			string message = data.GetString ("alert");
			Dictionary<string, string> messageData = new Dictionary<string,string> ();

			foreach (string key in data.KeySet()) {
				messageData.Add(key, data.GetString(key));	
			}

			(push.Registration as GcmRegistration).OnPushNotification (message, messageData);
		}

		protected abstract void DefaultHandleEvent (object sender, AeroGear.Push.PushReceivedEvent e);
	}
}

