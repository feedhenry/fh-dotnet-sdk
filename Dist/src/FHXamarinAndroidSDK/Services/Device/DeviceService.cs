using System;
using System.IO;
using AeroGear.Push;
using Android.App;
using Android.Provider;
using FHSDK.Services.Device;
using FHSDK.Services.Log;
using Java.Util;

namespace FHSDK.Services
{
    /// <summary>
    ///     Device info service provider for Android
    /// </summary>
    public class DeviceService : IDeviceService
    {
        private const string Tag = "FHSDK:DeviceService";
        private const string AppPropFile = "fhconfig.properties";
        private const string LocalDevAppPropFile = "fhconfig.local.properties";
        private const string ProjectIdProp = "projectid";
        private const string AppIdProp = "appid";
        private const string AppKeyProp = "appkey";
        private const string HostProp = "host";
        private const string ConnectionTagProp = "connectiontag";
        private const string ModeProp = "mode";
        private AppProps _appPropsObj;

        public string GetDeviceId()
        {
            return Settings.System.GetString(Application.Context.ContentResolver, Settings.Secure.AndroidId);
        }

        public AppProps ReadAppProps()
        {
            if (null != _appPropsObj) return _appPropsObj;
            var appProps = new Properties();
            Stream input = null;
            var logger = ServiceFinder.Resolve<ILogService>();
            try
            {
                bool foundLocalDevProps;
                try
                {
                    input = Application.Context.Assets.Open(LocalDevAppPropFile);
                    foundLocalDevProps = true;
                }
                catch (Exception)
                {
                    input = null;
                    foundLocalDevProps = false;
                    input = Application.Context.Assets.Open(AppPropFile);
                }
                appProps.Load(input);
                _appPropsObj = new AppProps
                {
                    projectid = appProps.GetProperty(ProjectIdProp),
                    appid = appProps.GetProperty(AppIdProp),
                    appkey = appProps.GetProperty(AppKeyProp),
                    host = appProps.GetProperty(HostProp),
                    connectiontag = appProps.GetProperty(ConnectionTagProp),
                    mode = appProps.GetProperty(ModeProp)
                };
                if (foundLocalDevProps)
                {
                    _appPropsObj.IsLocalDevelopment = true;
                }
                return _appPropsObj;
            }
            catch (Exception ex)
            {
                logger?.e(Tag, "Failed to load " + AppPropFile, ex);
                return null;
            }
            finally
            {
                input?.Dispose();
            }
        }

        public string GetDeviceDestination()
        {
            return "android";
        }

        public string GetPackageDir()
        {
            return "./";
        }

        public PushConfig ReadPushConfig()
        {
            var appProps = new Properties();
            Stream input = null;
            var logger = ServiceFinder.Resolve<ILogService>();
            try
            {
                input = Application.Context.Assets.Open(AppPropFile);
                appProps.Load(input);

                var pushServerUrl = appProps.GetProperty(HostProp) + "/api/v2/ag-push/";
                var pushSenderId = appProps.GetProperty("PUSH_SENDER_ID");
                var pushVariant = appProps.GetProperty("PUSH_VARIANT");
                var pushSecret = appProps.GetProperty("PUSH_SECRET");

                if (appProps.GetProperty(HostProp) != null && pushSenderId != null && pushVariant != null && pushSecret != null)
                    return new AndroidPushConfig
                    {
                        UnifiedPushUri = new Uri(pushServerUrl),
                        SenderId = pushSenderId,
                        VariantId = pushVariant,
                        VariantSecret = pushSecret
                    };
                var ex =
                    new InvalidOperationException(
                        "fhconfig.properties must define PUSH_SERVER_URL, PUSH_SENDER_ID, PUSH_VARIANT, and PUSH_SECRET.  One or more were not defined.");
                logger.e(Tag,
                    "fhconfig.properties must define PUSH_SERVER_URL, PUSH_SENDER_ID, PUSH_VARIANT, and PUSH_SECRET.  One or more were not defined.",
                    ex);
                throw ex;
            }
            catch (Exception ex)
            {
                logger?.e(Tag, "Failed to load " + AppPropFile, ex);
                return null;
            }
            finally
            {
                input?.Dispose();
            }
        }
    }
}