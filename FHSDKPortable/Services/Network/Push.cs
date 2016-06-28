using AeroGear.Push;

namespace FHSDK.Services.Network
{
    public class Push: PushBase
    {
        protected override RegistrationBase CreateRegistration()
        {
            return new Registration();
        }
    }
}
