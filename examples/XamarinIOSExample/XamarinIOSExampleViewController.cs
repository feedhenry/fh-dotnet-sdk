using System;
using System.Drawing;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using FHSDK.Services;
using FHSDK;
using System.Collections.Generic;
using FHSDK.Touch;

namespace XamarinIOSExample
{
	public partial class XamarinIOSExampleViewController : UIViewController
	{
		private const string COLLECTION_NAME = "Devices";

		static bool UserInterfaceIdiomIsPhone {
			get { return UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Phone; }
		}

		public XamarinIOSExampleViewController (IntPtr handle) : base (handle)
		{
		}

		public override void DidReceiveMemoryWarning ()
		{
			// Releases the view if it doesn't have a superview.
			base.DidReceiveMemoryWarning ();
			
			// Release any cached data, images, etc that aren't in use.
		}

		#region View lifecycle

		public async override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			FH.SetLogLevel ((int)LogService.LogLevels.DEBUG);
			// Perform any additional setup after loading the view, typically from a nib.
			await FHClient.Init ();
			ShowMessage ("App Ready!");
		}

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);
		}

		public override void ViewDidAppear (bool animated)
		{
			base.ViewDidAppear (animated);
		}

		public override void ViewWillDisappear (bool animated)
		{
			base.ViewWillDisappear (animated);
		}

		public override void ViewDidDisappear (bool animated)
		{
			base.ViewDidDisappear (animated);
		}

		#endregion

        async partial void onCloudCallTouched (UIButton sender)
		{
			Dictionary<string, object> data = new Dictionary<string, object>();
			data.Add("hello", "world");
			string message = null;
			FHResponse res = await FH.Cloud("hello", "GET", null, data);
			if(res.StatusCode == System.Net.HttpStatusCode.OK){
				message = (string) res.GetResponseAsDictionary()["msg"];
			} else {
				message = "Error";
			}

			ShowMessage(message);
		}

        async partial void onMbaasCallTouched (UIButton sender)
		{
			Dictionary<string, object> data = new Dictionary<string, object>();
			data.Add("act", "create");
			data.Add("type", COLLECTION_NAME);
			//create the collection first
			FHResponse createRes = await FH.Mbaas("db", data);
			ShowMessage(createRes.RawResponse);

			//read device id
			string deviceId = GetDeviceId();

			//check if it exists
			data = new Dictionary<string, object>();
			data.Add("type", COLLECTION_NAME);
			data.Add("act", "list");
			Dictionary<string, string> deviceIdField = new Dictionary<string, string>();
			deviceIdField.Add("deviceId", deviceId);
			data.Add("eq", deviceIdField);
			FHResponse listRes = await FH.Mbaas("db", data);
			ShowMessage(listRes.RawResponse);

			IDictionary<string, object> listResDic = listRes.GetResponseAsDictionary();
			if( Convert.ToInt16(listResDic["count"]) == 0 ){
				data = new Dictionary<string, object>();
				data.Add("act", "create");
				data.Add("type", COLLECTION_NAME);
				data.Add("fields", GetDeviceInfo());

				FHResponse dataCreateRes = await FH.Mbaas("db", data);
				ShowMessage(dataCreateRes.RawResponse);
			} else {
				ShowMessage("Device is already created!");
			}
		}

        async partial void onAuthCallTouched (UIButton sender)
		{
			string authPolicy = "TestGooglePolicy";
			FHResponse res = await FH.Auth(authPolicy);
			if(null == res.Error){
				ShowMessage(res.RawResponse);
			} else {
				ShowMessage(res.Error.Message);
			}

		}

		private void ShowMessage(string message)
		{
			this.messageField.Text = message;
		}

		private Dictionary<string, string> GetDeviceInfo()
		{
			Dictionary<string, string> info = new Dictionary<string, string> ();
			info.Add ("deviceId", GetDeviceId());
			info.Add ("device", UIDevice.CurrentDevice.Name);
			info.Add ("model", UIDevice.CurrentDevice.Model);
			info.Add ("manufacture", "Apple");
			info.Add ("product", UIDevice.CurrentDevice.Name);
			return info;
		}

		private string GetDeviceId()
		{
			IDeviceService deviceService = ServiceFinder.Resolve<IDeviceService>();
			string deviceId = deviceService.GetDeviceId();
			return deviceId;
		}
			
	}
}

