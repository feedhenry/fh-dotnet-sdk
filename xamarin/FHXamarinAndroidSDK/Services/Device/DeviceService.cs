using System;
using System.Threading.Tasks;
using Android.App;
using Java.IO;
using Java.Util;
using System.IO;
using FHSDK.Services.Device;
using FHSDK.Services.Log;
using System.Collections.Generic;
using AeroGear.Push;

namespace FHSDK.Services
{
    /// <summary>
    /// Device info service provider for Android
    /// </summary>
	public class DeviceService : IDeviceService
	{
		private const string TAG = "FHSDK:DeviceService";
		private const string APP_PROP_FILE = "fhconfig.properties";
        private const string LOCAL_DEV_APP_PROP_FILE = "fhconfig.local.properties";
		private const string PROJECT_ID_PROP = "projectid";
		private const string APP_ID_PROP = "appid";
		private const string APP_KEY_PROP = "appkey";
		private const string HOST_PROP = "host";
		private const string CONNECTION_TAG_PROP = "connectiontag";
		private const string MODE_PROP = "mode";
		private AppProps appPropsObj = null;

		public DeviceService  ()
		{
		}

		public String GetDeviceId()
		{
			return Android.Provider.Settings.System.GetString(Application.Context.ContentResolver, Android.Provider.Settings.Secure.AndroidId);
		}
			
		public AppProps ReadAppProps()
		{
			if (null == appPropsObj) {
				Properties appProps = new Properties();
				Stream input = null;
				ILogService logger = ServiceFinder.Resolve<ILogService>();
                bool foundLocalDevProps = false;
				try {
                    try
                    {
                        input = Application.Context.Assets.Open(LOCAL_DEV_APP_PROP_FILE);
                        foundLocalDevProps = true;
                    }
                    catch (Exception ex)
                    {
                        input = null;
                        foundLocalDevProps = false;
                        input = Application.Context.Assets.Open(APP_PROP_FILE);
                    }
					appProps.Load(input);
					appPropsObj = new AppProps();
					appPropsObj.projectid = appProps.GetProperty(PROJECT_ID_PROP);
					appPropsObj.appid = appProps.GetProperty(APP_ID_PROP);
					appPropsObj.appkey = appProps.GetProperty(APP_KEY_PROP);
					appPropsObj.host = appProps.GetProperty(HOST_PROP);
					appPropsObj.connectiontag = appProps.GetProperty(CONNECTION_TAG_PROP);
					appPropsObj.mode = appProps.GetProperty(MODE_PROP);
                    if(foundLocalDevProps){
                        appPropsObj.IsLocalDevelopment = true;
                    }
					return appPropsObj;
				} catch (Exception ex) {
					if(null != logger) {
						logger.e(TAG, "Failed to load " + APP_PROP_FILE, ex);
					}
					return null;

				} finally {
					if(null != input){
						try {
							input.Close();
						} catch (Exception exc) {
							if(null != logger){
								logger.w(TAG, "Failed to close stream", exc);
							}
						}
					}
				}
			}
			return appPropsObj;
		}

		public string GetDeviceDestination()
		{
			return "android";

		}

		public string GetPackageDir()
		{
			return "./";
		}

		public PushConfig ReadPushConfig ()
		{
			Properties appProps = new Properties();
			Stream input = null;
			ILogService logger = ServiceFinder.Resolve<ILogService>();
			try {
				input = Application.Context.Assets.Open(APP_PROP_FILE);
				appProps.Load(input);

				string pushServerUrl = appProps.GetProperty("PUSH_SERVER_URL");
				string pushSenderId = appProps.GetProperty("PUSH_SENDER_ID");
				string pushVariant = appProps.GetProperty("PUSH_VARIANT");
				string pushSecret = appProps.GetProperty("PUSH_SECRET");

				if (pushServerUrl == null || pushSenderId == null || pushVariant == null || pushSecret == null) {
					var ex = new System.InvalidOperationException ("fhconfig.properties must define PUSH_SERVER_URL, PUSH_SENDER_ID, PUSH_VARIANT, and PUSH_SECRET.  One or more were not defined.");
					logger.e(TAG, "fhconfig.properties must define PUSH_SERVER_URL, PUSH_SENDER_ID, PUSH_VARIANT, and PUSH_SECRET.  One or more were not defined.", ex);
					throw ex;
				}


				var config = new AndroidPushConfig();

				config.UnifiedPushUri  = new Uri( pushServerUrl ) ;
				config.SenderId = pushSenderId;
				config.VariantId = pushVariant;
				config.VariantSecret = pushSecret;

				return config;

			} catch (Exception ex) {
				if(null != logger) {
					logger.e(TAG, "Failed to load " + APP_PROP_FILE, ex);
				}
				return null;

			} finally {
				if(null != input){
					try {
						input.Close();
					} catch (Exception exc) {
						if(null != logger){
							logger.w(TAG, "Failed to close stream", exc);
						}
					}
				}
			}
		}

	}
}

