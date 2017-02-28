using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using FHSDK.Services.Auth;
using Microsoft.Phone.Controls;

namespace FHSDK.Services.Auth
{
    /// <summary>
    ///     OAuth login handler for windows phone
    /// </summary>
    internal class OAuthClientHandlerService : OAuthClientHandlerServiceBase
    {
        private TaskCompletionSource<OAuthResult> _tcs;
        private WebBrowser _webBrowser;

        public override Task<OAuthResult> Login(string oauthLoginUrl)
        {
            _tcs = new TaskCompletionSource<OAuthResult>();
            var uri = new Uri(oauthLoginUrl, UriKind.Absolute);
            Deployment.Current.Dispatcher.BeginInvoke(() =>
            {
                var frame = Application.Current.RootVisual as PhoneApplicationFrame;
                if (null != frame)
                {
                    var page = frame.Content as PhoneApplicationPage;
                    if (null != page)
                    {
                        var grid = page.FindName("LayoutRoot") as Grid;
                        if (null != grid)
                        {
                            _webBrowser = new WebBrowser {IsScriptEnabled = true};
                            _webBrowser.LoadCompleted += browser_LoadCompleted;
                            _webBrowser.NavigationFailed += browser_NavigateFailed;
                            _webBrowser.Navigating += browser_Navigating;
                            _webBrowser.Navigate(uri);
                            grid.Children.Add(_webBrowser);
                            page.BackKeyPress += backkey_Pressed;
                        }
                        else
                        {
                            _tcs.TrySetException(new Exception("Can not find RooLayout"));
                        }
                    }
                    else
                    {
                        _tcs.TrySetException(new Exception("Can not find ApplicationPage"));
                    }
                }
                else
                {
                    _tcs.TrySetException(new Exception("Can not find ApplicationFrame"));
                }
            });
            return _tcs.Task;
        }

        private void Close()
        {
            if (null != _webBrowser)
            {
                Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    var frame = Application.Current.RootVisual as PhoneApplicationFrame;
                    if (frame != null)
                    {
                        var page = frame.Content as PhoneApplicationPage;
                        if (page != null)
                        {
                            var grid = page.FindName("LayoutRoot") as Grid;
                            if (grid != null)
                            {
                                grid.Children.Remove(_webBrowser);
                            }
                            //page.ApplicationBar = null;
                            page.BackKeyPress -= backkey_Pressed;
                        }
                    }
                    _webBrowser = null;
                });
            }
        }

        private void backkey_Pressed(object sender, CancelEventArgs e)
        {
            Close();
            e.Cancel = true;
            var authResult = new OAuthResult(OAuthResult.ResultCode.Cancelled);
            _tcs.SetResult(authResult);
        }

        private void browser_LoadCompleted(object sender, NavigationEventArgs e)
        {
        }

        private void browser_Navigating(object sender, NavigatingEventArgs e)
        {
            var uri = e.Uri.ToString();
            if (uri.Contains("status=complete"))
            {
                OnSuccess(e.Uri, _tcs);
                Close();
            }
        }

        private void browser_NavigateFailed(object sender, NavigationFailedEventArgs e)
        {
            Close();
            var result = new OAuthResult(OAuthResult.ResultCode.Failed, e.Exception);
            _tcs.TrySetResult(result);
        }
    }
}