using AeroGear.Push;
using FHSDK.Services.Network;

namespace FHSDK.Services
{
    public class Push : PushBase
    {
        protected override RegistrationBase CreateRegistration()
        {
            return new GcmRegistration();
        }
    }
}