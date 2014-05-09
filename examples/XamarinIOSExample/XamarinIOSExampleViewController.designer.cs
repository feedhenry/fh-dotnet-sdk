// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using MonoTouch.Foundation;
using System.CodeDom.Compiler;

namespace XamarinIOSExample
{
	[Register ("XamarinIOSExampleViewController")]
	partial class XamarinIOSExampleViewController
	{
		[Outlet]
		MonoTouch.UIKit.UIButton authCallButton { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIButton cloudCallButton { get; set; }

		[Outlet]
		MonoTouch.UIKit.UIButton mbaasCallButton { get; set; }

		[Outlet]
		MonoTouch.UIKit.UITextView messageField { get; set; }

		[Action ("onAuthCallTouched:")]
		partial void onAuthCallTouched (MonoTouch.Foundation.NSObject sender);

		[Action ("onCloudCallTouched:")]
		partial void onCloudCallTouched (MonoTouch.Foundation.NSObject sender);

		[Action ("onMbaasCallTouched:")]
		partial void onMbaasCallTouched (MonoTouch.Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (authCallButton != null) {
				authCallButton.Dispose ();
				authCallButton = null;
			}

			if (cloudCallButton != null) {
				cloudCallButton.Dispose ();
				cloudCallButton = null;
			}

			if (mbaasCallButton != null) {
				mbaasCallButton.Dispose ();
				mbaasCallButton = null;
			}

			if (messageField != null) {
				messageField.Dispose ();
				messageField = null;
			}
		}
	}
}
