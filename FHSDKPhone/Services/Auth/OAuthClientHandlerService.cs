using FHSDK.Services;
using Microsoft.Phone.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace FHSDK.Services
{
    class OAuthClientHandlerService : OAuthClientHandlerServiceBase
    {
        private WebBrowser webBrowser;
        private TaskCompletionSource<OAuthResult> tcs;

        public OAuthClientHandlerService()
            : base()
        {

        }

        public override Task<OAuthResult> Login(string oauthLoginUrl)
        {
            tcs = new TaskCompletionSource<OAuthResult>();
            Uri uri = new Uri(oauthLoginUrl, UriKind.Absolute);
            Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                PhoneApplicationFrame frame = Application.Current.RootVisual as PhoneApplicationFrame;
                if (null != frame)
                {
                    PhoneApplicationPage page = frame.Content as PhoneApplicationPage;
                    if (null != page)
                    {
                        Grid grid = page.FindName("LayoutRoot") as Grid;
                        if (null != grid)
                        {
                            webBrowser = new WebBrowser();
                            webBrowser.IsScriptEnabled = true;
                            webBrowser.LoadCompleted += new System.Windows.Navigation.LoadCompletedEventHandler(browser_LoadCompleted);
                            webBrowser.NavigationFailed += new System.Windows.Navigation.NavigationFailedEventHandler(browser_NavigateFailed);
                            webBrowser.Navigating += new EventHandler<NavigatingEventArgs>(browser_Navigating);
                            webBrowser.Navigate(uri);
                            grid.Children.Add(webBrowser);
                            page.BackKeyPress += new EventHandler<CancelEventArgs>(backkey_Pressed);
                        }
                        else
                        {
                            tcs.TrySetException(new Exception("Can not find RooLayout"));
                        }
                    }
                    else
                    {
                        tcs.TrySetException(new Exception("Can not find ApplicationPage"));
                    }
                }
                else
                {
                    tcs.TrySetException(new Exception("Can not find ApplicationFrame"));
                }
            });
            return tcs.Task;
        }

        private void Close()
        {
            if (null != webBrowser)
            {
                Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    PhoneApplicationFrame frame = Application.Current.RootVisual as PhoneApplicationFrame;
                    if (frame != null)
                    {
                        PhoneApplicationPage page = frame.Content as PhoneApplicationPage;
                        if (page != null)
                        {
                            Grid grid = page.FindName("LayoutRoot") as Grid;
                            if (grid != null)
                            {
                                grid.Children.Remove(webBrowser);
                            }
                            //page.ApplicationBar = null;
                            page.BackKeyPress -= backkey_Pressed;
                        }
                    }
                    webBrowser = null;
                });
            }
        }

        void backkey_Pressed(object sender, CancelEventArgs e)
        {
            Close();
            e.Cancel = true;
            OAuthResult authResult = new OAuthResult(OAuthResult.ResultCode.CANCELLED);
            tcs.SetResult(authResult);
        }

        void browser_LoadCompleted(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {
        }

        void browser_Navigating(object sender, NavigatingEventArgs e)
        {
            string uri = e.Uri.ToString();
            if (uri.Contains("status=complete"))
            {
                base.OnSuccess(e.Uri, tcs);
                Close();
            }
        }

        void browser_NavigateFailed(object sender, System.Windows.Navigation.NavigationFailedEventArgs e)
        {
            Close();
            OAuthResult result = new OAuthResult(OAuthResult.ResultCode.FAILED, e.Exception);
            tcs.TrySetResult(result);
        }
    }
}
