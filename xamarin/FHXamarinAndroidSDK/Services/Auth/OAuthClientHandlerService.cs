using System;
using System.Threading.Tasks;
using Android.Content;
using Android.App;
using Android.OS;
using FHSDK.Services.Auth;
using FHSDK.Services.Log;

namespace FHSDK.Services
{
    /// <summary>
    /// OAuth login handler implementation for Android
    /// </summary>
	public class OAuthClientHandlerService: OAuthClientHandlerServiceBase
	{
		private Context appContext;
		private ILogService logger;
		private const string LOG_TAG = "XamarinAndroid:OAuthClientHandlerService";
		protected TaskCompletionSource<OAuthResult> tcs;
		private OAuthURLRedirectReceiver receiver;

		public OAuthClientHandlerService ()
			:base()
		{
			appContext = Application.Context;
			tcs = new TaskCompletionSource<OAuthResult> ();
			logger = ServiceFinder.Resolve<ILogService> ();
		}

		public override Task<OAuthResult> Login(string oauthLoginUrl)
		{
			logger.d (LOG_TAG, "Got oauth url = " + oauthLoginUrl, null);
			Bundle data = new Bundle ();
			data.PutString ("url", oauthLoginUrl);
			data.PutString ("title", "Login");
			Intent i = new Intent (this.appContext, typeof(FHOAuthIntent));
			receiver = new OAuthURLRedirectReceiver (this);
			IntentFilter filter = new IntentFilter (FHOAuthWebview.BROADCAST_ACTION_FILTER);
			this.appContext.RegisterReceiver (this.receiver, filter);
			i.PutExtra ("settings", data);
			i.AddFlags (ActivityFlags.NewTask);
			this.appContext.StartActivity (i);
			return tcs.Task;
		}

		class OAuthURLRedirectReceiver: BroadcastReceiver
		{
			private OAuthClientHandlerService parent;

			public OAuthURLRedirectReceiver()
				:base()
			{

			}

			public OAuthURLRedirectReceiver(OAuthClientHandlerService p)
				:base()
			{
				this.parent = p;
			}


			public override void OnReceive(Context context, Intent intent)
			{
				this.parent.logger.d (OAuthClientHandlerService.LOG_TAG, "recieved event, data: " + intent.GetStringExtra ("url"), null);
				string data = intent.GetStringExtra ("url");
				if ("NOT_FINISHED".Equals (data)) {
					OAuthResult oauthResult = new OAuthResult (OAuthResult.ResultCode.Failed, new Exception ("NOT_FINISHED"));
					this.parent.tcs.TrySetResult (oauthResult);
				} else if ("CANCELLED".Equals (data)) {
					OAuthResult oauthResult = new OAuthResult (OAuthResult.ResultCode.Cancelled, new Exception ("USER_CANCELLED"));
					this.parent.tcs.TrySetResult (oauthResult);
				} else {
					this.parent.appContext.UnregisterReceiver (this.parent.receiver);
					OnSuccess (new Uri (data), this.parent.tcs);
				}
			}
		}
	}
		
}

