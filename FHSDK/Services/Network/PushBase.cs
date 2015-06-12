using AeroGear.Push;
using System;
using System.Threading.Tasks;

namespace FHSDK.Services.Network
{
    public abstract class PushBase : IPush
    {
        public async Task<PushConfig> ReadConfig(Registration registration)
        {
            string configName;
            if (FHConfig.getInstance().IsLocalDevelopment)
            {
                configName = Constants.LOCAL_CONFIG_FILE_NAME;
            }
            else
            {
                configName = Constants.CONFIG_FILE_NAME;
            }

            return await registration.LoadConfigJson(configName);
        }

        public abstract Task Register(EventHandler<PushReceivedEvent> HandleNotification);
    }
}
