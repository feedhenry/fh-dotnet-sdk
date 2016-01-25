using System;
using System.Threading.Tasks;
using System.Diagnostics.Contracts;
using FHSDK.Services.Auth;
using UIKit;

namespace FHSDK.Services
{
    /// <summary>
    /// OAuth login handler implementation for iOS
    /// </summary>
	public class OAuthClientHandlerService: OAuthClientHandlerServiceBase
	{
		private TaskCompletionSource<OAuthResult> tcs;

		public OAuthClientHandlerService ()
			:base()
		{
			this.tcs = new TaskCompletionSource<OAuthResult> ();
		}

		public override Task<OAuthResult> Login(string oauthLoginUrl)
		{
			Contract.Assert (null != oauthLoginUrl);
			FHOAuthViewController controller = new FHOAuthViewController (oauthLoginUrl, this);
			this.GetTopViewController ().PresentViewController (controller, true, null);
			return this.tcs.Task;
		}

		private UIViewController GetTopViewController()
		{
			UIViewController topController = UIApplication.SharedApplication.KeyWindow.RootViewController;
			while (null != topController.PresentedViewController) {
				topController = topController.PresentedViewController;
			}
			return topController;
		}

		public void OnFail(bool cancelled){
			OAuthResult oauthResult = new OAuthResult (OAuthResult.ResultCode.Failed, new Exception ("NOT_FINISHED"));
			if (cancelled) {
				oauthResult = new OAuthResult (OAuthResult.ResultCode.Cancelled, new Exception ("USER_CANCELLED"));
			}
			this.tcs.TrySetResult (oauthResult);
		}

		public void OnSuccess(Uri uri)
		{
			OnSuccess (uri, tcs);
		}


	}
}

