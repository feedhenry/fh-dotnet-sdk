using System.Collections.Generic;
using System.Threading.Tasks;
using AeroGear.Push;
using Android.App;
using Android.Gms.Gcm;
using Android.Gms.Gcm.Iid;
using Android.OS;
using FHSDK.Services.Device;
using Task = System.Threading.Tasks.Task;

namespace FHSDK.Services
{
    public class GcmRegistration : RegistrationBase
    {
        private AndroidPushConfig _config;
        private Installation _installation;

        public override Task<PushConfig> LoadConfigJson(string filename)
        {
            return Task.Run(() => ServiceFinder.Resolve<IDeviceService>().ReadPushConfig());
        }

        protected override Installation CreateInstallation(PushConfig pushConfig)
        {
            _config = (AndroidPushConfig) pushConfig;
            return _installation = new Installation
            {
                alias = pushConfig.Alias,
                categories = pushConfig.Categories,
                operatingSystem = "android",
                osVersion = Build.VERSION.Release
            };
        }

        protected override ILocalStore CreateChannelStore()
        {
            return new AndroidStore();
        }

        protected override Task<string> ChannelUri()
        {
            return Task.Run(() =>
            {
                var token = InstanceID.GetInstance(Application.Context)
                    .GetToken(_config.SenderId, GoogleCloudMessaging.InstanceIdScope, new Bundle());

                return token;
            });
        }

        public override async Task<string> Register(PushConfig pushConfig, IUPSHttpClient client)
        {
            Installation installation = CreateInstallation(pushConfig);
            ILocalStore store = CreateChannelStore();
            string channelUri = await ChannelUri();
            var token = pushConfig.VariantId + channelUri;
            installation.deviceToken = channelUri;
            await client.Register(installation);
            return installation.deviceToken;
        }

        public void OnPushNotification(string message, Dictionary<string, string> messageData)
        {
            base.OnPushNotification(message, messageData);
        }
    }
}