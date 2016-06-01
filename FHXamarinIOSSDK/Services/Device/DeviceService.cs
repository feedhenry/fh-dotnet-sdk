using System;
using System.Collections.Generic;
using AdSupport;
using AeroGear.Push;
using FHSDK.Services.Device;
using FHSDK.Services.Log;
using Foundation;
using ObjCRuntime;
using Security;
using UIKit;

namespace FHSDK.Services
{
    /// <summary>
    ///     Device info service provider for iOS
    /// </summary>
    public class DeviceService : IDeviceService
    {
        private const string AppPropsFile = "fhconfig";
        private const string LocalDevAppPropsFile = "fhconfiglocal";
        private AppProps _appProps;
        private bool _localDev;
        private readonly ILogService _logger = ServiceFinder.Resolve<ILogService>();

        public string GetDeviceId()
        {
            if (!UIDevice.CurrentDevice.CheckSystemVersion(7, 0)) return UniqueID();
            var idManager = ASIdentifierManager.SharedManager;
            var uuid = idManager.AdvertisingIdentifier;
            return uuid.AsString();
        }


        public AppProps ReadAppProps()
        {
            if (null != _appProps) return _appProps;
            var props = ReadProperties();
            _appProps = new AppProps
            {
                host = ((NSString) props["host"]).ToString(),
                appid = ((NSString) props["appid"])?.ToString(),
                appkey = ((NSString) props["appkey"])?.ToString(),
                projectid = ((NSString) props["projectid"])?.ToString(),
                connectiontag = ((NSString) props["connectiontag"])?.ToString(),
                mode = ((NSString) props["mode"])?.ToString()
            };
            if (_localDev)
            {
                _appProps.IsLocalDevelopment = true;
            }
            return _appProps;
        }

        public PushConfig ReadPushConfig()
        {
            var config = ReadProperties();
            List<string> categoryList = null;
            var categories = (NSArray) config["categories"];
            if (categories != null)
            {
                categoryList = new List<string>();
                for (uint i = 0; i < categories.Count; i++)
                {
                    categoryList.Add(Runtime.GetNSObject(categories.ValueAt(i)).ToString());
                }
            }
            return new PushConfig
            {
                Alias = (NSString) config["alias"],
                Categories = categoryList,
                UnifiedPushUri = new Uri(ReadAppProps().host + "/api/v2/ag-push"),
                VariantId = (NSString) config["variantID"],
                VariantSecret = (NSString) config["variantSecret"]
            };
        }

        public string GetDeviceDestination()
        {
            return "ios";
        }

        public string GetPackageDir()
        {
            return "./";
        }

        private NSDictionary ReadProperties()
        {
            var path = NSBundle.MainBundle.PathForResource(LocalDevAppPropsFile, "plist");
            if (null == path)
            {
                path = NSBundle.MainBundle.PathForResource(AppPropsFile, "plist");
            }
            else
            {
                _localDev = true;
            }
            return NSDictionary.FromFile(path);
        }

        //a simple way to generate and persist unique id for ios device using keychain. This unique id won't change even if app is uninstalled and re-installed.
        //only for devices pre ios 6 (from here: http://david-smith.org/iosversionstats/, only < 5% devices)
        private string UniqueID()
        {
            var query = new SecRecord(SecKind.GenericPassword);
            query.Service = NSBundle.MainBundle.BundleIdentifier;
            query.Account = "UniqueID";

            var uniqueId = SecKeyChain.QueryAsData(query);
            if (uniqueId == null)
            {
                query.ValueData = NSData.FromString(Guid.NewGuid().ToString());
                var err = SecKeyChain.Add(query);
                if (err != SecStatusCode.Success && err != SecStatusCode.DuplicateItem)
                {
                    _logger.i("IOSDeviceService", "Failed to save unique id", null);
                }

                return query.ValueData.ToString();
            }
            return uniqueId.ToString();
        }
    }
}