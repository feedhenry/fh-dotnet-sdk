using System;
using System.Collections.Generic;
using System.Net;
using System.Windows;
using FHSDK;
using FHSDK.Phone;
using FHSDK.Services;
using FHSDK.Services.Device;
using Microsoft.Phone.Info;
using Environment = Microsoft.Devices.Environment;
using AeroGear.Push;
using FHSDK.Services.Network;

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
            FHClient.RegisterPush(NotificationReceived);
            ShowMessage("App Ready!");
        }

        void NotificationReceived(object sender, PushReceivedEvent pushEvent)
        {
            Dispatcher.BeginInvoke(() =>
            {
                ShowMessage(pushEvent.Args.Message);
            });
        }

        private async void CloudButton_Click(object sender, RoutedEventArgs e)
        {
            var data = new Dictionary<string, object>();
            data.Add("hello", "world");
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
            var authPolicy = "TestGooglePolicy";
            var res = await FH.Auth(authPolicy);
            if (null == res.Error)
            {
                ShowMessage(res.RawResponse);
            }
            else
            {
                ShowMessage(res.Error.Message);
            }
        }

        private async void MBAASAuthButton_Click(object sender, RoutedEventArgs e)
        {
            var authPolicy = "LdapTest";
            var username = "Martin Murphy";
            var password = "hello";
            var res = await FH.Auth(authPolicy, username, password);
            if (null == res.Error)
            {
                ShowMessage(res.RawResponse);
            }
            else
            {
                ShowMessage(res.Error.Message);
            }
        }

        private async void MBAASButton_Click(object sender, RoutedEventArgs e)
        {
            var data = new Dictionary<string, object>();
            data.Add("act", "create");
            data.Add("type", CollectionName);
            //create the collection first
            var createRes = await FH.Mbaas("db", data);
            ShowMessage(createRes.RawResponse);

            //read device id
            var deviceId = GetDeviceId();

            //check if it exists
            data = new Dictionary<string, object>();
            data.Add("type", CollectionName);
            data.Add("act", "list");
            var deviceIdField = new Dictionary<string, string>();
            deviceIdField.Add("deviceId", deviceId);
            data.Add("eq", deviceIdField);
            var listRes = await FH.Mbaas("db", data);
            ShowMessage(listRes.RawResponse);

            var listResDic = listRes.GetResponseAsDictionary();
            if (Convert.ToInt16(listResDic["count"]) == 0)
            {
                data = new Dictionary<string, object>();
                data.Add("act", "create");
                data.Add("type", CollectionName);
                data.Add("fields", GetDeviceInfo());

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
            var info = new Dictionary<string, string>();
            info.Add("deviceId", GetDeviceId());
            info.Add("device", Environment.DeviceType.ToString());
            info.Add("model", DeviceStatus.DeviceHardwareVersion);
            info.Add("manufacture", DeviceStatus.DeviceManufacturer);
            info.Add("product", DeviceStatus.DeviceName);
            return info;
        }

        private string GetDeviceId()
        {
            var deviceService = ServiceFinder.Resolve<IDeviceService>();
            var deviceId = deviceService.GetDeviceId();
            return deviceId;
        }
    }
}