using System.Linq;
using AeroGear.Push;
using Android.Gms.Gcm;
using Android.OS;
using FHSDK.Services.Network;

namespace FHSDK.Services
{
    public abstract class FeedHenryMessageReceiver : GcmListenerService
    {
        private const string DefaultMessageHandlerKey = "DEFAULT_MESSAGE_HANDLER_KEY";

        public override async void OnMessageReceived(string from, Bundle data)
        {
            if (!ServiceFinder.IsRegistered<IPush>())
            {
                await FHClient.Init();
                FH.RegisterPush(DefaultHandleEvent);
            }

            var push = ServiceFinder.Resolve<IPush>() as Push;
            var message = data.GetString("alert");
            var messageData = data.KeySet().ToDictionary(key => key, key => data.GetString(key));

            (push.Registration as GcmRegistration).OnPushNotification(message, messageData);
        }

        protected abstract void DefaultHandleEvent(object sender, PushReceivedEvent e);
    }
}