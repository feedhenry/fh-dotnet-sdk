using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using FHSDK;
using FHSDK.FHHttpClient;
using System.Diagnostics;

namespace FHSDKExampleApp.View
{
    public partial class AuthDemoView : UserControl
    {
        public AuthDemoView()
        {
            InitializeComponent();
        }

        private async void OAuth_Click(object sender, RoutedEventArgs e)
        {
            FHResponse authRes = await FH.Auth("MyGooglePolicy");
            if (null == authRes.Error)
            {
                Debug.WriteLine("OAuth logged in");
                MessageBox.Show("User logged in. Res = " + authRes.RawResponse);
            }
            else
            {
                Debug.WriteLine("OAuth failed");
                MessageBox.Show("OAuth failed. Code = " + authRes.Error.Error + ". Message = " + authRes.Error.Message);
            }
        }

        private async void FHAuth_Click(object sender, RoutedEventArgs e)
        {
            FHResponse authRes = await FH.Auth("MyFeedHenryPolicy", "wptest", "Password1");
            if (null == authRes.Error)
            {
                MessageBox.Show("User logged in");
            }
            else
            {
                MessageBox.Show("Login failed");
            }
        }
    }
}
