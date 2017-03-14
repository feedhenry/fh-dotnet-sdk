using System;
using Android.App;
using Android.OS;
using Android.Views;

namespace FHSDK.Services
{
    /// <summary>
    /// Start a new intent for OAuth login using WebView.
    /// </summary>
	[Activity (Name="fhsdk.services.FHOAuthIntent")]
	public class FHOAuthIntent: Activity
	{
		private FHOAuthWebview oauthWebView;

		public FHOAuthIntent ()
			:base()
		{

		}

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);
			this.Window.RequestFeature (Android.Views.WindowFeatures.NoTitle);
			this.Window.SetFlags (Android.Views.WindowManagerFlags.ForceNotFullscreen, Android.Views.WindowManagerFlags.ForceNotFullscreen);
			oauthWebView = new FHOAuthWebview (this, this.Intent.GetBundleExtra ("settings"));
			oauthWebView.onCreate ();
			this.SetContentView (oauthWebView.GetView ());
		}

		public override bool OnKeyDown(Keycode keyCode, KeyEvent e)
		{
			if (keyCode == Keycode.Back) {
				oauthWebView.close (true);
				return true;
			} else {
				return base.OnKeyDown (keyCode, e);
			}
		}
	}
}

