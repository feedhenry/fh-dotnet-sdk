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
    class OAuthClientHandlerService : IOAuthClientHandlerService
    {
        private WebBrowser webBrowser;
        private TaskCompletionSource<OAuthResult> tcs;

        public Task<OAuthResult> Login(string oauthLoginUrl)
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
                            page.ApplicationBar = null;
                        }
                    }
                    webBrowser = null;
                });
            }
        }

        void backkey_Pressed(object sender, CancelEventArgs e)
        {
            Close();
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
                string queryParams = e.Uri.Query;
                string[] parts = queryParams.Split('&');
                Dictionary<string, string> queryMap = new Dictionary<string, string>();
                for (int i = 0; i < parts.Length; i++)
                {
                    string[] kv = parts[i].Split('=');
                    queryMap.Add(kv[0], kv[1]);
                }

                string result = null;
                queryMap.TryGetValue("result", out result);
                Close();
                if ("success" == result)
                {
                    string sessionToken = null;
                    string authRes = null;
                    queryMap.TryGetValue("fh_auth_session", out sessionToken);
                    queryMap.TryGetValue("authResponse", out authRes);
                    OAuthResult oauthResult = new OAuthResult(OAuthResult.ResultCode.OK, sessionToken, Uri.UnescapeDataString(authRes));
                    tcs.TrySetResult(oauthResult);
                }
                else
                {
                    string errorMessage = null;
                    queryMap.TryGetValue("message", out errorMessage);
                    OAuthResult oauthResult = new OAuthResult(OAuthResult.ResultCode.FAILED, new Exception(errorMessage));
                    tcs.TrySetResult(oauthResult);
                }
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
