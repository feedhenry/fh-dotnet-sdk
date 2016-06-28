using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using AeroGear.Push;
using FHSDK.Services.Device;
using FHSDK.Services.Log;

namespace FHSDK.Services.Network
{
    /// <summary>
    ///     PushBase implements IPush interface and provides default implementations
    ///     for setting aliases/categories and reading config file.
    /// </summary>
    public abstract class PushBase : IPush
    {
        private const string LogTag = "Push";
        private readonly ILogService _logger;

		public RegistrationBase Registration { get; private set; }

        /// <summary>
        ///     Default constructor.
        /// </summary>
        protected PushBase()
        {
            _logger = ServiceFinder.Resolve<ILogService>();
        }

        /// <summary>
        ///     Method to register to push notifications.
        /// </summary>
        /// <param name="handleNotification">Attached handler triggered when push notifications are received.</param>
        /// <returns></returns>
        public async Task Register(EventHandler<PushReceivedEvent> handleNotification)
        {
			if (Registration == null) {
				Registration = CreateRegistration ();
			}

            Registration.PushReceivedEvent += handleNotification;

            try
            {
                var config = ReadConfig();
                await Registration.Register(config);
            }
            catch (SerializationException)
            {
                _logger.e(LogTag,
                    "push configuration not found skipping push register, update fhconfig with UPS details", null);
            }
        }

        /// <summary>
        ///     Associate a category to filter the push notifications.
        /// </summary>
        /// <param name="categories"></param>
        /// <returns></returns>
        public async Task SetCategories(List<string> categories)
        {
			if (Registration == null) {
				Registration = CreateRegistration ();
			}
			var config = ReadConfig();
            config.Categories = categories;
            await Registration.UpdateConfig(config);
        }

        /// <summary>
        ///     Associate an alias to filter push notifications.
        /// </summary>
        /// <param name="alias"></param>
        /// <returns></returns>
        public async Task SetAlias(string alias)
        {
			if (Registration == null) {
				Registration = CreateRegistration ();
			}

			var config = ReadConfig();
            config.Alias = alias;
            await Registration.UpdateConfig(config);
        }

        /// <summary>
        ///     Read a config file for a given rgistration. Defaulted to fhconfig.json.
        ///     In development mode, fhconfig.local.json overrides default config file.
        /// </summary>
        /// <returns>The push config</returns>
        internal static PushConfig ReadConfig()
        {
            return ServiceFinder.Resolve<IDeviceService>().ReadPushConfig();
        }

        /// <summary>
        ///     Abstract method to create a registration.
        /// </summary>
        /// <returns></returns>
        protected abstract RegistrationBase CreateRegistration();
    }
}