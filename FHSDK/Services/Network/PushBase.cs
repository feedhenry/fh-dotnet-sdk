using AeroGear.Push;
using System.Threading.Tasks;

namespace FHSDK.Services.Network
{
    public abstract class PushBase : IPush
    {
        public async Task ReadConfig(Registration registration)
        {
            string configName;
            if (FHConfig.getInstance().IsLocalDevelopment)
            {
                configName = IDeviceService.LOCAL_CONFIG_FILE_NAME;
            }
            else
            {
                configName = IDeviceService.CONFIG_FILE_NAME;
            }

            await registration.LoadConfigJson(configName);
        }
    }
}
