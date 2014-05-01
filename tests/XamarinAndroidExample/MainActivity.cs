using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using FHSDK.Droid;
using FHSDK.Services;



namespace XamarinAndroidExample
{
	[Activity (Label = "XamarinAndroidExample", MainLauncher = true)]
	public class MainActivity : Activity
	{
		int count = 1;

		protected async override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			// Set our view from the "main" layout resource
			SetContentView (Resource.Layout.Main);
			await  FH.Init();
		}
	}
}


