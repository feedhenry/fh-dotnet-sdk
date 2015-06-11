using AeroGear.Push;
using System;
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
                configName = Constants.LOCAL_CONFIG_FILE_NAME;
            }
            else
            {
                configName = Constants.CONFIG_FILE_NAME;
            }

            await registration.LoadConfigJson(configName);
        }

        public abstract Task Register(EventHandler<PushReceivedEvent> HandleNotification);
    }
}
