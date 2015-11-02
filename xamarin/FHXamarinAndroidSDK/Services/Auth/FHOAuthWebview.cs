using System;
using Android.Webkit;
using Android.OS;
using Android.App;
using Android.Content;
using Android.Views;
using Android.Widget;
using Android.Graphics;
using FHSDK.Services.Log;

namespace FHSDK.Services
{
    /// <summary>
    /// OAuth login handler Webview
    /// </summary>
	public class FHOAuthWebview
	{
		private WebView webView;
		private Bundle webSettings;
		private Activity context;
		private ViewGroup mainLayout;
		protected string finishedUrl = "NOT_FINISHED";
		protected bool finished = false;
		protected const string LOG_TAG = "FHOAuthWebview";
		protected ILogService logger;
		public const string BROADCAST_ACTION_FILTER = "com.feedhenry.sdk.oauth.urlChanged";

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="aContext">application context</param>
        /// <param name="aSettings">setting</param>
		public FHOAuthWebview (Activity aContext, Bundle aSettings)
		{
			this.context = aContext;
			this.webSettings = aSettings;
			this.logger = ServiceFinder.Resolve<ILogService> ();
		}

        /// <summary>
        /// Construct the UI and start loading
        /// </summary>
		public void onCreate()
		{
			string startUrl = webSettings.GetString ("url");
			string title = webSettings.GetString ("title");
			mainLayout = new LinearLayout (this.context);
			LinearLayout.LayoutParams lp = new LinearLayout.LayoutParams (ViewGroup.LayoutParams.FillParent, ViewGroup.LayoutParams.FillParent, 0.0F);
			mainLayout.LayoutParameters = lp;
			((LinearLayout)mainLayout).SetGravity (GravityFlags.CenterVertical);
			((LinearLayout)mainLayout).Orientation = Orientation.Vertical;

			webView = new WebView (this.context);
			WebSettings settings = webView.Settings;
			settings.JavaScriptEnabled = true;
			settings.BuiltInZoomControls = true;
			settings.JavaScriptCanOpenWindowsAutomatically = true;

			webView.LayoutParameters = new LinearLayout.LayoutParams (ViewGroup.LayoutParams.FillParent, ViewGroup.LayoutParams.FillParent, 1.0F);

			webView.SetWebViewClient (new FHOAuthWebViewClient (this));
			webView.RequestFocusFromTouch ();
			webView.Visibility = ViewStates.Visible;

			LinearLayout barlayout = initHeaderBar (title);

			mainLayout.AddView (barlayout);
			mainLayout.SetBackgroundColor (Color.Transparent);
			mainLayout.SetBackgroundResource (0);
			mainLayout.AddView (this.webView);

			this.webView.LoadUrl (startUrl);

		}

		private LinearLayout initHeaderBar(string title)
		{
			LinearLayout barlayout = new LinearLayout (this.context);
			LinearLayout.LayoutParams blp = new LinearLayout.LayoutParams (ViewGroup.LayoutParams.FillParent, ViewGroup.LayoutParams.WrapContent, 0.0F);
			barlayout.LayoutParameters = blp;
			barlayout.SetGravity (GravityFlags.CenterHorizontal | GravityFlags.CenterVertical);
			barlayout.SetBackgroundColor (Color.Black);

			TextView text = new TextView (this.context);
			if (!title.Equals ("undefined")) {
				text.Text = title;
			}
			text.SetTextColor (Color.White);
			text.Gravity = GravityFlags.CenterHorizontal | GravityFlags.CenterVertical;
			text.TextSize = 20F;
			text.LayoutParameters = new LinearLayout.LayoutParams (ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent, 1.0F);
			barlayout.AddView (text);
			return barlayout;
		}

        /// <summary>
        /// Close the WebView
        /// </summary>
        /// <param name="cancelled">if the action is cancelled by the user</param>
		public void close(bool cancelled = false)
		{
			this.webView.StopLoading ();
			Intent i = new Intent ();
			i.SetAction (BROADCAST_ACTION_FILTER);
			string message = cancelled ? "CANCELLED" : finishedUrl;
			i.PutExtra ("url", message);
			this.context.SendBroadcast (i);
			this.context.Finish ();
		}

        /// <summary>
        /// Return the root view
        /// </summary>
        /// <returns></returns>
		public ViewGroup GetView()
		{
			return mainLayout;
		}

        /// <summary>
        /// Destroy the view
        /// </summary>
		public void Destroy()
		{
			if (null != this.webView) {
				this.webView.Destroy ();
			}
		}

		class FHOAuthWebViewClient : WebViewClient
		{
			private FHOAuthWebview parent;
			public FHOAuthWebViewClient()
				:base()
			{

			}

			public FHOAuthWebViewClient(FHOAuthWebview parent)
				:base()
			{
				this.parent = parent;
			}


			public override bool ShouldOverrideUrlLoading(WebView view, string url)
			{
				this.parent.logger.d (FHOAuthWebview.LOG_TAG, "going to load url " + url, null);
				Uri uri = new Uri (url);
				if (uri.Scheme.Contains ("http")) {
					return false;
				}
				return true;
			}

			public override void OnPageStarted(WebView view, string url, Bitmap favicon)
			{
				this.parent.logger.d (FHOAuthWebview.LOG_TAG, "start to load " + url, null);
				Uri uri = new Uri (url);
				string query = uri.Query;
				if (query.IndexOf ("status=complete") > -1) {
					this.parent.finished = true;
					this.parent.finishedUrl = url;
				}
			}

			public override void OnPageFinished(WebView view, string url)
			{
				this.parent.logger.d (FHOAuthWebview.LOG_TAG, "finish loading " + url, null);
				if (this.parent.finished && !"about:blank".Equals (url)) {
					this.parent.close ();
				}
			}

			public override void OnReceivedError(WebView view, ClientError errorCode, string description, string failingUrl)
			{
				this.parent.logger.d (FHOAuthWebview.LOG_TAG, "error: " + description + " url: " + failingUrl, null);
			}


		}
	}



}

