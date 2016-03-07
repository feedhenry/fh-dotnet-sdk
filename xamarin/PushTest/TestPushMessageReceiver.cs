using System;
using FHSDK.Services;
using Android.Util;
using Android.App;

namespace PushTest
{
	[Service (Name="fhsdk.Services.TestPushMessageReceiver", Exported = false)]
	[IntentFilter(new String[]{"com.google.android.c2dm.intent.RECEIVE"})]
	public class TestPushMessageReceiver : FeedHenryMessageReceiver
	{
		public TestPushMessageReceiver ()
		{
		}

		protected override void DefaultHandleEvent (object sender, AeroGear.Push.PushReceivedEvent e)
		{
			Log.Info ("MESSAGE", "Test " + e.Args.Message);
		}

	}
}

