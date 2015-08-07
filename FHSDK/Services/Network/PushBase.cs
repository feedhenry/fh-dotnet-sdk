using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using AeroGear.Push;
using FHSDK.Config;
using FHSDK.Services.Device;
using FHSDK.Services.Log;

namespace FHSDK.Services.Network
{
    public abstract class PushBase : IPush
    {
        private const string LogTag = "Push";
        private readonly ILogService _logger;

        protected PushBase()
        {
            _logger = ServiceFinder.Resolve<ILogService>();
        }

        public async Task Register(EventHandler<PushReceivedEvent> handleNotification)
        {
            var registration = CreateRegistration();
            registration.PushReceivedEvent += handleNotification;

            try
            {
                var config = await ReadConfig(registration);
                await registration.Register(config);
            }
            catch (SerializationException)
            {
                _logger.e(LogTag,
                    "push configuration not found skipping push register, update fhconfig with UPS details", null);
            }
        }

        public async Task SetCategories(List<string> categories)
        {
            var registration = CreateRegistration();
            var config = await ReadConfig(registration);
            config.Categories = categories;
            await registration.UpdateConfig(config);
        }

        public async Task SetAlias(string alias)
        {
            var registration = CreateRegistration();
            var config = await ReadConfig(registration);
            config.Alias = alias;
            await registration.UpdateConfig(config);
        }

        private async Task<PushConfig> ReadConfig(Registration registration)
        {
            string configName;
            configName = FHConfig.GetInstance().IsLocalDevelopment ? Constants.LocalConfigFileName : Constants.ConfigFileName;

            return await registration.LoadConfigJson(configName);
        }

        protected abstract Registration CreateRegistration();
    }
}