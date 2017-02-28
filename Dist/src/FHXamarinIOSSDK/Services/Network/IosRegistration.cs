using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AeroGear.Push;
using Foundation;
using UIKit;

namespace FHSDK.Services
{
    public class IosRegistration : RegistrationBase
    {
        private readonly AutoResetEvent _waitHandle = new AutoResetEvent(false);
        private string _token;

        public IosRegistration()
        {
            NSNotificationCenter.DefaultCenter.AddObserver(new NSString("sucess_registered"),
                obj => { OnRegisteredForRemoteNotifications((NSData) obj.Object); });

            NSNotificationCenter.DefaultCenter.AddObserver(new NSString("message_received"), obj =>
            {
                var data = (NSDictionary) obj.Object;
                var message = (NSDictionary) data["aps"];
                var alert = message["alert"];
                if (alert is NSDictionary)
                {
                    alert = ((NSDictionary) alert)["body"];
                }
                OnPushNotification(alert.ToString(), Convert(message));
            });
        }

        private static Dictionary<string, string> Convert(NSDictionary nativeDict)
        {
            return nativeDict.ToDictionary<KeyValuePair<NSObject, NSObject>, string, string>(
                item => (NSString) item.Key, item => item.Value.ToString());
        }

        private void OnRegisteredForRemoteNotifications(NSData deviceToken)
        {
            _token = deviceToken.Description;
            if (!string.IsNullOrWhiteSpace(_token))
            {
                _token = _token.Trim('<').Trim('>');
            }

            _waitHandle.Set();
        }

        protected override Installation CreateInstallation(PushConfig pushConfig)
        {
            var device = new UIDevice();
            return new Installation
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
            return Task.Run(() =>
            {
                _waitHandle.WaitOne();
                return _token;
            });
        }
    }
}