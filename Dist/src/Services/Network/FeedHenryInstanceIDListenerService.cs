using Android.App;
using Android.Gms.Gcm.Iid;
using FHSDK.Services.Device;
using FHSDK.Services.Network;

namespace FHSDK.Services
{
    /// <summary>
    ///     Feed henry instance identifier listener service.  If the InstanceID changes this service class will automatically
    ///     reregister.
    /// </summary>
    [Service(Name = "fhsdk.Services.FeedHenryInstanceIDListenerService", Exported = false),
     IntentFilter(new[] {"com.google.android.gms.iid.InstanceID"})]
    public class FeedHenryInstanceIDListenerService : InstanceIDListenerService
    {
        public override void OnTokenRefresh()
        {
            if (ServiceFinder.IsRegistered<IPush>())
            {
                var push = ServiceFinder.Resolve<IPush>() as Push;
                var config = ServiceFinder.Resolve<IDeviceService>().ReadPushConfig();

                push.Registration.UpdateConfig(config);
            }
        }
    }
}