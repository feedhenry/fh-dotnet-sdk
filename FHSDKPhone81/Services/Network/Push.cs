using AeroGear.Push;
using FHSDK.Services.Network;
using System;
using System.Threading.Tasks;

namespace FHSDKPhone.Services.Network
{
    public class Push: PushBase
    {
        public override async Task Register(EventHandler<PushReceivedEvent> HandleNotification)
        {
            Registration registration = new WnsRegistration();
            registration.PushReceivedEvent += HandleNotification;

            PushConfig config = await ReadConfig(registration);
            await registration.Register(config);
        }
    }
}
