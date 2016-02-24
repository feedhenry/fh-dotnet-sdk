using System;
using System.Threading.Tasks;
using FHSDK.Services.Network;
using AeroGear.Push;
using Foundation;
using UIKit;

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
            return Task.Run(() => CreateChannelStore().Read("PushDeviceToken"));
	    }

	    public void OnRegisteredForRemoteNotifications(NSData deviceToken)
	    {
	        var storage = CreateChannelStore();
            var token = deviceToken.Description;
            if (!string.IsNullOrWhiteSpace(token))
            {
                token = token.Trim('<').Trim('>');
            }

            storage.Save("PushDeviceToken", token);
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

