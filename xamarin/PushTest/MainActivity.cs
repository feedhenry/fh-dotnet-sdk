using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

using System.Threading.Tasks;
using FHSDK;
using Android.Util;

namespace PushTest
{
	[Activity (Label = "PushTest", MainLauncher = true, Icon = "@mipmap/icon")]
	public class MainActivity : Activity
	{



		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);
		}


		protected override void OnPause ()
		{
			base.OnPause ();
			((PushTestApplication)Application).MainThreadPushEvent -= DisplayToast;
		}

		protected override void OnResume ()
		{
			base.OnResume ();
			((PushTestApplication)Application).MainThreadPushEvent += DisplayToast;
		}

		void DisplayToast (object sender, AeroGear.Push.PushReceivedEvent e)
		{
			Toast.MakeText (this, e.Args.Message, ToastLength.Long).Show ();
		}
	}
}



