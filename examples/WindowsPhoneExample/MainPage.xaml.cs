using System;
using System.Collections.Generic;
using System.Net;
using System.Windows;
using AeroGear.Push;
using FHSDK;
using FHSDK.Services;
using FHSDK.Services.Device;
using Microsoft.Phone.Info;
using Environment = Microsoft.Devices.Environment;

namespace WindowsPhoneExample
{
    public partial class MainPage
    {
        private const string CollectionName = "Devices";
        // Constructor
        public MainPage()
        {
            InitializeComponent();
            InitApp();
        }

        private async void InitApp()
        {
            await FHClient.Init();
            FH.RegisterPush(NotificationReceived);
            ShowMessage("App Ready!");
        }

        private void NotificationReceived(object sender, PushReceivedEvent pushEvent)
        {
            Dispatcher.BeginInvoke(() => { ShowMessage(pushEvent.Args.Message); });
        }

        private async void CloudButton_Click(object sender, RoutedEventArgs e)
        {
            var data = new Dictionary<string, object> {{"hello", "world"}};
            string message;
            var res = await FH.Cloud("hello", "GET", null, data);
            if (res.StatusCode == HttpStatusCode.OK)
            {
                message = (string) res.GetResponseAsDictionary()["msg"];
            }
            else
            {
                message = "Error";
            }

            ShowMessage(message);
        }

        private void ShowMessage(string message)
        {
            textField.Text = message;
        }

        private async void AuthButton_Click(object sender, RoutedEventArgs e)
        {
            const string authPolicy = "TestGooglePolicy";
            var res = await FH.Auth(authPolicy);
            ShowMessage(null == res.Error ? res.RawResponse : res.Error.Message);
        }

        private async void MBAASAuthButton_Click(object sender, RoutedEventArgs e)
        {
            const string authPolicy = "LdapTest";
            const string username = "Martin Murphy";
            const string password = "hello";
            var res = await FH.Auth(authPolicy, username, password);
            ShowMessage(null == res.Error ? res.RawResponse : res.Error.Message);
        }

        private async void MBAASButton_Click(object sender, RoutedEventArgs e)
        {
            var data = new Dictionary<string, object> {{"act", "create"}, {"type", CollectionName}};
            //create the collection first
            var createRes = await FH.Mbaas("db", data);
            ShowMessage(createRes.RawResponse);

            //read device id
            var deviceId = GetDeviceId();

            //check if it exists
            data = new Dictionary<string, object> {{"type", CollectionName}, {"act", "list"}};
            var deviceIdField = new Dictionary<string, string> {{"deviceId", deviceId}};
            data.Add("eq", deviceIdField);
            var listRes = await FH.Mbaas("db", data);
            ShowMessage(listRes.RawResponse);

            var listResDic = listRes.GetResponseAsDictionary();
            if (Convert.ToInt16(listResDic["count"]) == 0)
            {
                data = new Dictionary<string, object>
                {
                    {"act", "create"},
                    {"type", CollectionName},
                    {"fields", GetDeviceInfo()}
                };

                var dataCreateRes = await FH.Mbaas("db", data);
                ShowMessage(dataCreateRes.RawResponse);
            }
            else
            {
                ShowMessage("Device is already created!");
            }
        }

        private Dictionary<string, string> GetDeviceInfo()
        {
            var info = new Dictionary<string, string>
            {
                {"deviceId", GetDeviceId()},
                {"device", Environment.DeviceType.ToString()},
                {"model", DeviceStatus.DeviceHardwareVersion},
                {"manufacture", DeviceStatus.DeviceManufacturer},
                {"product", DeviceStatus.DeviceName}
            };
            return info;
        }

        private static string GetDeviceId()
        {
            var deviceService = ServiceFinder.Resolve<IDeviceService>();
            var deviceId = deviceService.GetDeviceId();
            return deviceId;
        }
    }
}