using System;
using System.Threading.Tasks;
using FHSDK.Services.Network;
using AeroGear.Push;
using Foundation;
using UIKit;
using System.Threading;

namespace FHSDK.Services
{
	public class Push: PushBase
	{
		protected override Registration CreateRegistration() 
		{
			if (UIDevice.CurrentDevice.CheckSystemVersion (8, 0)) {
				var notificationSettings = UIUserNotificationSettings.GetSettingsForTypes (UIUserNotificationType.Sound |
					UIUserNotificationType.Alert | UIUserNotificationType.Badge, null);

				UIApplication.SharedApplication.RegisterUserNotificationSettings (notificationSettings);
				UIApplication.SharedApplication.RegisterForRemoteNotifications ();
			} else {
				UIApplication.SharedApplication.RegisterForRemoteNotificationTypes(UIRemoteNotificationType.Badge |
					UIRemoteNotificationType.Sound | UIRemoteNotificationType.Alert);
			}

			return new IosRegistration();
		}
	}

	public class IosRegistration: Registration {
		private AutoResetEvent _waitHandle = new AutoResetEvent(false);
		private string token;

		public IosRegistration() 
		{
			NSNotificationCenter.DefaultCenter.AddObserver (new NSString("sucess_registered"), (NSNotification obj) => {
				OnRegisteredForRemoteNotifications((NSData) obj.Object);
			});
		}
			
		private void OnRegisteredForRemoteNotifications(NSData deviceToken)
		{
			token = deviceToken.Description;
			if (!string.IsNullOrWhiteSpace(token))
			{
				token = token.Trim('<').Trim('>');
			}

			_waitHandle.Set();
		}

	    protected override Installation CreateInstallation(PushConfig pushConfig)
	    {
            var device = new UIDevice();
	        return new Installation()
	        {
	            alias = pushConfig.Alias,
	            categories = pushConfig.Categories,
	            operatingSystem = device.SystemName,
	            osVersion = device.SystemVersion
	        };
	    }

	    protected override ILocalStore CreateChannelStore()
	    {
	        return new IosStorage();
	    }

	    public override Task<PushConfig> LoadConfigJson(string filename)
	    {
	        throw new NotImplementedException();
	    }

	    protected override Task<string> ChannelUri()
	    {			
			return Task.Run(() => {
				_waitHandle.WaitOne();
				return token;
			});
	    }
    }

    public class IosStorage : ILocalStore
    {

        public string Read(string key)
        {
            return NSUserDefaults.StandardUserDefaults.StringForKey(key);
        }

        public void Save(string key, string value)
        {
            NSUserDefaults.StandardUserDefaults.SetString(value, key);
        }
    }
}

