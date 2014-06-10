// WARNING
//
// This file has been generated automatically by Xamarin Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using System;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using System.CodeDom.Compiler;

namespace XamarinIOSExample
{
	[Register ("XamarinIOSExampleViewController")]
	partial class XamarinIOSExampleViewController
	{
		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIButton authCallButton { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIButton cloudCallButton { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIButton mbaasCallButton { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UITextView messageField { get; set; }

		[Action ("onAuthCallTouched:")]
		[GeneratedCode ("iOS Designer", "1.0")]
		partial void onAuthCallTouched (UIButton sender);

		[Action ("onCloudCallTouched:")]
		[GeneratedCode ("iOS Designer", "1.0")]
		partial void onCloudCallTouched (UIButton sender);

		[Action ("onMbaasCallTouched:")]
		[GeneratedCode ("iOS Designer", "1.0")]
		partial void onMbaasCallTouched (UIButton sender);

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
