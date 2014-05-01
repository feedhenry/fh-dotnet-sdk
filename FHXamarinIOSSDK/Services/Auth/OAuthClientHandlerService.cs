using System;
using MonoTouch.UIKit;
using System.Threading.Tasks;
using System.Diagnostics.Contracts;

namespace FHSDK.Services
{
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

		public void OnFail(){
			OAuthResult oauthResult = new OAuthResult (OAuthResult.ResultCode.FAILED, new Exception ("NOT_FINISHED"));
			this.tcs.TrySetResult (oauthResult);
		}

		public void OnSuccess(Uri uri)
		{
			this.OnSuccess (uri, tcs);
		}


	}
}

