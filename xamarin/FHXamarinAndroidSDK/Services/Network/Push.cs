using AeroGear.Push;
using FHSDK.Services.Network;

namespace FHSDK.Services
{
    public class Push : PushBase
    {
        protected override Registration CreateRegistration()
        {
            return new GcmRegistration();
        }
    }
}