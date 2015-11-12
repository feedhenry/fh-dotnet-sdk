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
    /// <summary>
    /// PushBase implements IPush interface and provides default implementations 
    /// for setting aliases/categories and reading config file.
    /// </summary>
    public abstract class PushBase : IPush
    {
        private const string LogTag = "Push";
        private readonly ILogService _logger;

        /// <summary>
        /// Default constructor.
        /// </summary>
        protected PushBase()
        {
            _logger = ServiceFinder.Resolve<ILogService>();
        }

        /// <summary>
        /// Method to register to push notifications.
        /// </summary>
        /// <param name="handleNotification">Attached handler triggered when push notifications are received.</param>
        /// <returns></returns>
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

        /// <summary>
        /// Associate a category to filter the push notifications.
        /// </summary>
        /// <param name="categories"></param>
        /// <returns></returns>
        public async Task SetCategories(List<string> categories)
        {
            var registration = CreateRegistration();
            var config = await ReadConfig(registration);
            config.Categories = categories;
            await registration.UpdateConfig(config);
        }

        /// <summary>
        /// Associate an alias to filter push notifications.
        /// </summary>
        /// <param name="alias"></param>
        /// <returns></returns>
        public async Task SetAlias(string alias)
        {
            var registration = CreateRegistration();
            var config = await ReadConfig(registration);
            config.Alias = alias;
            await registration.UpdateConfig(config);
        }

        /// <summary>
        /// Read a config file for a given rgistration. Defaulted to fhconfig.json. 
        /// In development mode, fhconfig.local.json overrides default config file.
        /// </summary>
        /// <param name="registration"></param>
        /// <returns></returns>
        private async Task<PushConfig> ReadConfig(Registration registration)
        {
            string configName;
            configName = FHConfig.GetInstance().IsLocalDevelopment ? Constants.LocalConfigFileName : Constants.ConfigFileName;

            return await registration.LoadConfigJson(configName);
        }

        /// <summary>
        /// Abstract method to create a registration.
        /// </summary>
        /// <returns></returns>
        protected abstract Registration CreateRegistration();
    }
}