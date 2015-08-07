using AeroGear.Push;
using FHSDK.Services.Network;
using System;
using System.Threading.Tasks;

namespace FHSDKPhone.Services.Network
{
    public class Push: PushBase
    {
        protected override Registration CreateRegistration()
        {
            return new WnsRegistration();
        }
    }
}
