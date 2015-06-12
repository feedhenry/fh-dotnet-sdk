using AeroGear.Push;
using System;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace FHSDK.Services.Network
{
    public abstract class PushBase : IPush
    {
        private ILogService logger;
        const string LOG_TAG = "Push";

        public PushBase()
        {
            logger = ServiceFinder.Resolve<ILogService>();
        }

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

        public async Task Register(EventHandler<PushReceivedEvent> HandleNotification)
        {
            Registration registration = CreateRegistration();
            registration.PushReceivedEvent += HandleNotification;

            try
            {
                PushConfig config = await ReadConfig(registration);
                await registration.Register(config);

            }
            catch (SerializationException e)
            {
                logger.e(LOG_TAG, "push configuration not found skipping push register, update fhconfig with UPS details", null);
            }
        }

        public abstract Registration CreateRegistration();
    }
}
