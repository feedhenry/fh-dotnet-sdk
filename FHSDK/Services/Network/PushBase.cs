using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using AeroGear.Push;
using FHSDK.Config;
using FHSDK.Services.Data;
using FHSDK.Services.Device;
using FHSDK.Services.Log;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;

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
		public async Task<Registration> Register(EventHandler<PushReceivedEvent> handleNotification)
        {
            var registration = CreateRegistration();
            registration.PushReceivedEvent += handleNotification;

            try
            {
                var config = ReadConfig();
                await registration.Register(config);
            }
            catch (SerializationException)
            {
                _logger.e(LogTag,
                    "push configuration not found skipping push register, update fhconfig with UPS details", null);
            }

			return registration;
        }

        /// <summary>
        /// Associate a category to filter the push notifications.
        /// </summary>
        /// <param name="categories"></param>
        /// <returns></returns>
        public async Task SetCategories(List<string> categories)
        {
            var registration = CreateRegistration();
            var config = ReadConfig();
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
            var config = ReadConfig();
            config.Alias = alias;
            await registration.UpdateConfig(config);
        }

        /// <summary>
        /// Read a config file for a given rgistration. Defaulted to fhconfig.json. 
        /// In development mode, fhconfig.local.json overrides default config file.
        /// </summary>
        /// <returns>The push config</returns>
        internal static PushConfig ReadConfig()
        {
            var configName = FHConfig.GetInstance().IsLocalDevelopment ? Constants.LocalConfigFileName : Constants.ConfigFileName;

            var deviceService = ServiceFinder.Resolve<IDeviceService>();
            var appProps = deviceService.ReadAppProps();
			var configLocation = Path.Combine(deviceService.GetPackageDir(), configName);
            var json = ServiceFinder.Resolve<IIOService>().ReadFile(configLocation);
            var config = (JObject)JsonConvert.DeserializeObject(json);
            var configWindows = config["windows"];

            var pushConfig = new PushConfig()
            {
                Alias = (string) config["Alias"],
                Categories = config["Categories"].ToObject<List<string>>(),
                UnifiedPushUri = new Uri(appProps.host + "/api/v2/ag-push"),
                VariantId = (string)(configWindows != null ? configWindows["variantID"] : config["variantID"]),
                VariantSecret = (string)(configWindows != null ? configWindows["variantSecret"] : config["variantSecret"])
            };

            return pushConfig;
        }

        /// <summary>
        /// Abstract method to create a registration.
        /// </summary>
        /// <returns></returns>
        protected abstract Registration CreateRegistration();
    }
}