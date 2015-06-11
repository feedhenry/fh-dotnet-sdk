using AeroGear.Push;
using FHSDK.Services.Network;
using System;
using System.Threading.Tasks;

namespace FHSDKPhone.Services.Network
{
    public class Push: PushBase
    {
        public async Task Register(EventHandler<PushReceivedEvent> HandleNotification)
        {
            Registration registration = new WnsRegistration();
            registration.PushReceivedEvent += HandleNotification;

            await ReadConfig(registration);
            await registration.Register();
        }
    }
}
