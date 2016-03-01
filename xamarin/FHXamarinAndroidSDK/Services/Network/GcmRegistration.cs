using System;
using AeroGear.Push;
using System.Threading.Tasks;
using Android.App;
using Android.Gms.Gcm.Iid;
using Android.Gms.Gcm;
using System.Collections.Generic;
using FHSDK.Services.Device;


namespace FHSDK.Services
{
	public class GcmRegistration : Registration
	{

		private AndroidPushConfig Config;
		private Installation Installation;
		public GcmRegistration ()
		{
		}

		public override System.Threading.Tasks.Task<PushConfig> LoadConfigJson (string filename)
		{
			return System.Threading.Tasks.Task.Run (() => {
				return	ServiceFinder.Resolve<IDeviceService> ().ReadPushConfig ();
			});
		}

		protected override Installation CreateInstallation (PushConfig pushConfig)
		{
			Config = (AndroidPushConfig) pushConfig;
			return Installation = new Installation
			{
				alias = pushConfig.Alias,
				categories = pushConfig.Categories,
				operatingSystem = "android",
				osVersion = Android.OS.Build.VERSION.Release
			};
		}

		protected override ILocalStore CreateChannelStore ()
		{
			return new AndroidStore();
		}

		protected override System.Threading.Tasks.Task<string> ChannelUri ()
		{
			return System.Threading.Tasks.Task.Run (() => {
				return InstanceID.GetInstance (Application.Context).GetToken(Config.SenderId, GoogleCloudMessaging.InstanceIdScope, null);
			});
		}

		public void OnPushNotification (string message, Dictionary<string, string> messageData)
		{
			base.OnPushNotification (message, messageData);
		}
	}
}

