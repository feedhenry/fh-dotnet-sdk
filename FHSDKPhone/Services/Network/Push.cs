using AeroGear.Push;
using FHSDK.Services.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FHSDKPhone.Services.Network
{
    public class Push: IPush
    {
        public async Task Register(EventHandler<PushReceivedEvent> HandleNotification)
        {
            Registration registration = new MpnsRegistration();
            registration.PushReceivedEvent += HandleNotification;
            await registration.LoadConfigJson("");
            await registration.Register();
        }
    }
}
