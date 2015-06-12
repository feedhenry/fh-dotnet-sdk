using AeroGear.Push;
using FHSDK.Services.Network;
using System;
using System.Threading.Tasks;

namespace FHSDKPhone.Services.Network
{
    public class Push: PushBase
    {
        public override Registration CreateRegister()
        {
            return new WnsRegistration();
        }
    }
}
