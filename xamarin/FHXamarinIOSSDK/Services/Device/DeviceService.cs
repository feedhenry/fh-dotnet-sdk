using System;
using System.Threading.Tasks;
using FHSDK.Services.Device;
using FHSDK.Services.Log;
using Security;
using Foundation;
using UIKit;
using AdSupport;
using AeroGear.Push;
using System.Collections.Generic;
using System.Collections;
using ObjCRuntime;

namespace FHSDK.Services
{
    /// <summary>
    /// Device info service provider for iOS
    /// </summary>
	public class DeviceService : IDeviceService
	{
		private ILogService logger;
		private AppProps appProps;
		private bool localDev;
		private const string APP_PROPS_FILE = "fhconfig";
        private const string LOCAL_DEV_APP_PROPS_FILE = "fhconfiglocal";

		public DeviceService ()
		{
			logger = ServiceFinder.Resolve<ILogService> ();
		}

		public String GetDeviceId()
		{
			if (UIDevice.CurrentDevice.CheckSystemVersion (7, 0)) {
				ASIdentifierManager idManager = ASIdentifierManager.SharedManager;
				NSUuid uuid = idManager.AdvertisingIdentifier;
				return uuid.AsString ();
			} else {
				return UniqueID ();
			}
		}
			

		public AppProps ReadAppProps()
		{
			if (null == appProps) {
				NSDictionary props = ReadProperties ();
				appProps = new AppProps ();
				appProps.host = ((NSString) props["host"]).ToString();
				appProps.appid = null == props["appid"]? null : ((NSString)props ["appid"]).ToString();
				appProps.appkey = null == props["appkey"]? null : ((NSString)props ["appkey"]).ToString();
				appProps.projectid = null == props ["projectid"] ? null : ((NSString)props ["projectid"]).ToString();
				appProps.connectiontag = null == props ["connectiontag"] ? null : ((NSString)props ["connectiontag"]).ToString();
				appProps.mode = null == props ["mode"] ? null : ((NSString)props ["mode"]).ToString();
                if(localDev){
                    appProps.IsLocalDevelopment = true;
                }
			}
			return appProps;
		}

		public PushConfig ReadPushConfig() 
		{
			var config = ReadProperties ();
			List<string> categoryList = null;
			var categories = (NSArray)config ["categories"];
			if (categories != null) {
				categoryList = new List<string> ();
				for (uint i = 0; i < categories.Count; i++) {
					categoryList.Add (Runtime.GetNSObject (categories.ValueAt (i)).ToString ());
				}
			}
			return new PushConfig()
			{
				Alias = (NSString) config["alias"],
				Categories = categoryList,
				//UnifiedPushUri = new Uri(ReadAppProps().host + "/api/v2/ag-push"),
				UnifiedPushUri = new Uri("https://unifiedpush-edewit.rhcloud.com/ag-push/"),
				VariantId = (NSString) config["variantID"],
				VariantSecret = (NSString) config["variantSecret"]
			};

		}

		private NSDictionary ReadProperties() 
		{
			string path = NSBundle.MainBundle.PathForResource (LOCAL_DEV_APP_PROPS_FILE, "plist");
			if(null == path){
				path = NSBundle.MainBundle.PathForResource(APP_PROPS_FILE, "plist");
			} else {
				localDev = true;
			}
			return NSDictionary.FromFile (path);
		}

		public string GetDeviceDestination()
		{
			return "ios";
		}

		public string GetPackageDir()
		{
			return "./";
		}

		//a simple way to generate and persist unique id for ios device using keychain. This unique id won't change even if app is uninstalled and re-installed.
		//only for devices pre ios 6 (from here: http://david-smith.org/iosversionstats/, only < 5% devices)
		private string UniqueID() {
			var query = new SecRecord(SecKind.GenericPassword);
			query.Service = NSBundle.MainBundle.BundleIdentifier;
			query.Account = "UniqueID";

			NSData uniqueId = SecKeyChain.QueryAsData(query);
			if(uniqueId == null) {
				query.ValueData = NSData.FromString(System.Guid.NewGuid().ToString());
				var err = SecKeyChain.Add (query);
				if (err != SecStatusCode.Success && err != SecStatusCode.DuplicateItem) {
					logger.i ("IOSDeviceService", "Failed to save unique id", null); 
				}

				return query.ValueData.ToString();
			}
			else {
				return uniqueId.ToString();
			}
		}

	}
}

