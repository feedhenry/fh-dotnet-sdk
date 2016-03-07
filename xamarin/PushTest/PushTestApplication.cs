using System;
using Android.App;
using FHSDK;
using System.Threading.Tasks;
using Android.OS;
using Android.Widget;
using AeroGear.Push;
using Android.Util;
using Android.Runtime;

namespace PushTest
{
	[Application (Name="pushTest.PushTestApplication")]
	public class PushTestApplication : Application
	{
		public event EventHandler<PushReceivedEvent> MainThreadPushEvent;

		public PushTestApplication (IntPtr handle, JniHandleOwnership transfer)
			: base(handle,transfer)
		{
		}

		public async override void OnCreate ()
		{
			base.OnCreate ();
			await Task.Run (async ()=>{
				await FHClient.Init ();
				FH.RegisterPush(HandleEvent);
			});
		}

		private void HandleEvent (object sender, AeroGear.Push.PushReceivedEvent e)
		{
			if (MainThreadPushEvent != null && MainThreadPushEvent.GetInvocationList ().Length > 0) 
			{
				Delegate[] ds = MainThreadPushEvent.GetInvocationList ();
				foreach (Delegate d in ds) {	
					new Handler (Looper.MainLooper).Post (() => {
						d.DynamicInvoke (sender, e);
					});
				}

			}
			else
			{
				Log.Info ("PUSH", e.Args.Message);
			}

		}

	}
}

