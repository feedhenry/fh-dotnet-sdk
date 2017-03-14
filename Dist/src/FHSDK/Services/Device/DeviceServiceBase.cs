using System;
using System.Collections.Generic;
using System.IO;
using AeroGear.Push;
using FHSDK.Config;
using FHSDK.Services.Data;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace FHSDK.Services.Device
{
    public abstract class DeviceServiceBase : IDeviceService
    {
        public abstract string GetDeviceId();
        public abstract AppProps ReadAppProps();

        public PushConfig ReadPushConfig()
        {
            var configName = FHConfig.GetInstance().IsLocalDevelopment
                ? Constants.LocalConfigFileName
                : Constants.ConfigFileName;

            var appProps = ReadAppProps();
            var configLocation = Path.Combine(GetPackageDir(), configName);
            var json = ServiceFinder.Resolve<IIOService>().ReadFile(configLocation);
            var config = (JObject) JsonConvert.DeserializeObject(json);
            var configWindows = config["windows"];

            var pushConfig = new PushConfig
            {
                Alias = (string) config["Alias"],
                Categories = config["Categories"]?.ToObject<List<string>>(),
                UnifiedPushUri = new Uri(appProps.host + "/api/v2/ag-push"),
                VariantId = (string) (configWindows != null ? configWindows["variantID"] : config["variantID"]),
                VariantSecret =
                    (string) (configWindows != null ? configWindows["variantSecret"] : config["variantSecret"])
            };

            return pushConfig;
        }

        public abstract string GetDeviceDestination();
        public abstract string GetPackageDir();
    }
}