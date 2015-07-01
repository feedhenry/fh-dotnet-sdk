using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using WindowsPhoneExample.Resources;
using FHSDK.Phone;
using FHSDK;
using FHSDK.Services;
using Microsoft.Phone.Info;
using Microsoft.Devices;
using AeroGear.Push;
using FHSDK.Services.Network;

namespace WindowsPhoneExample
{
    public partial class MainPage : PhoneApplicationPage
    {
        private const string COLLECTION_NAME = "Devices";

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
                ShowMessage(pushEvent.Args.message);
            });
        }

        private async void CloudButton_Click(object sender, RoutedEventArgs e)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data.Add("hello", "world");
            string message = null;
            FHResponse res = await FH.Cloud("hello", "GET", null, data);
            if (res.StatusCode == System.Net.HttpStatusCode.OK)
            {
                message = (string)res.GetResponseAsDictionary()["msg"];
            }
            else
            {
                message = "Error";
            }

            ShowMessage(message);
        }



        private void ShowMessage(string message)
        {
            this.textField.Text = message;
        }

        private async void AuthButton_Click(object sender, RoutedEventArgs e)
        {
            string authPolicy = "TestGooglePolicy";
            FHResponse res = await FH.Auth(authPolicy);
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
            string authPolicy = "LdapTest";
            string username = "Martin Murphy";
            string password = "hello";
            FHResponse res = await FH.Auth(authPolicy, username, password);
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
            Dictionary<string, object> data = new Dictionary<string, object>();
            data.Add("act", "create");
            data.Add("type", COLLECTION_NAME);
            //create the collection first
            FHResponse createRes = await FH.Mbaas("db", data);
            ShowMessage(createRes.RawResponse);

            //read device id
            string deviceId = GetDeviceId();

            //check if it exists
            data = new Dictionary<string, object>();
            data.Add("type", COLLECTION_NAME);
            data.Add("act", "list");
            Dictionary<string, string> deviceIdField = new Dictionary<string, string>();
            deviceIdField.Add("deviceId", deviceId);
            data.Add("eq", deviceIdField);
            FHResponse listRes = await FH.Mbaas("db", data);
            ShowMessage(listRes.RawResponse);

            IDictionary<string, object> listResDic = listRes.GetResponseAsDictionary();
            if (Convert.ToInt16(listResDic["count"]) == 0)
            {
                data = new Dictionary<string, object>();
                data.Add("act", "create");
                data.Add("type", COLLECTION_NAME);
                data.Add("fields", GetDeviceInfo());

                FHResponse dataCreateRes = await FH.Mbaas("db", data);
                ShowMessage(dataCreateRes.RawResponse);
            }
            else
            {
                ShowMessage("Device is already created!");
            }
        }

        private Dictionary<string, string> GetDeviceInfo()
        {
            Dictionary<string, string> info = new Dictionary<string, string>();
            info.Add("deviceId", GetDeviceId());
            info.Add("device", Microsoft.Devices.Environment.DeviceType.ToString());
            info.Add("model", DeviceStatus.DeviceHardwareVersion);
            info.Add("manufacture", DeviceStatus.DeviceManufacturer);
            info.Add("product", DeviceStatus.DeviceName);
            return info;
        }

        private string GetDeviceId()
        {
            IDeviceService deviceService = ServiceFinder.Resolve<IDeviceService>();
            string deviceId = deviceService.GetDeviceId();
            return deviceId;
        }


        // Sample code for building a localized ApplicationBar
        //private void BuildLocalizedApplicationBar()
        //{
        //    // Set the page's ApplicationBar to a new instance of ApplicationBar.
        //    ApplicationBar = new ApplicationBar();

        //    // Create a new button and set the text value to the localized string from AppResources.
        //    ApplicationBarIconButton appBarButton = new ApplicationBarIconButton(new Uri("/Assets/AppBar/appbar.add.rest.png", UriKind.Relative));
        //    appBarButton.Text = AppResources.AppBarButtonText;
        //    ApplicationBar.Buttons.Add(appBarButton);

        //    // Create a new menu item with the localized string from AppResources.
        //    ApplicationBarMenuItem appBarMenuItem = new ApplicationBarMenuItem(AppResources.AppBarMenuItemText);
        //    ApplicationBar.MenuItems.Add(appBarMenuItem);
        //}
    }
}