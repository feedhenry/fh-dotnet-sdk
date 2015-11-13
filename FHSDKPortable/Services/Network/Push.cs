using AeroGear.Push;

namespace FHSDK.Services.Network
{
    public class Push: PushBase
    {
        protected override Registration CreateRegistration()
        {
            return new WnsRegistration();
        }
    }
}
