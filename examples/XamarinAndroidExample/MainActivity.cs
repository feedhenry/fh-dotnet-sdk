using System;
using System.Collections.Generic;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using FHSDK;
using FHSDK.Services;
using FHSDK.FHHttpClient;
using System.Net.Http;
using ModernHttpClient;
using FHSDK.Droid;

namespace XamarinAndroidExample
{
	[Activity (Label = "XamarinAndroidExample", MainLauncher = true)]
	public class MainActivity : Activity
	{
		private const string COLLECTION_NAME = "Devices";

		protected async override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			FH.SetLogLevel ((int)LogService.LogLevels.DEBUG);
			//use ModernHttpClient
			FHHttpClientFactory.Get = (() => new HttpClient(new OkHttpNetworkHandler()));

			// Set our view from the "main" layout resource
			SetContentView (Resource.Layout.Main);
			await  FHClient.Init();

			ShowMessage ("App Ready!");

			Button cloudButton = (Button) FindViewById (Resource.Id.button1);
			Button authButton = (Button) FindViewById (Resource.Id.button2);
			Button mbaasButton = (Button) FindViewById (Resource.Id.button3);
			cloudButton.Click += async (object sender, EventArgs e) => {
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
			};

			authButton.Click += async (object sender, EventArgs e) => {
				string authPolicy = "TestGooglePolicy";
				FHResponse res = await FH.Auth(authPolicy);
				ShowMessage(res.RawResponse);
			};

			mbaasButton.Click += async (object sender, EventArgs e) => {
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



			};

		}

		private void ShowMessage(string message)
		{
			TextView tv = (TextView) FindViewById (Resource.Id.textView1);


			tv.Text = message;
		}

		private Dictionary<string, string> GetDeviceInfo()
		{
			Dictionary<string, string> info = new Dictionary<string, string> ();
			info.Add ("deviceId", GetDeviceId());
			info.Add ("device", Android.OS.Build.Device);
			info.Add ("model", Android.OS.Build.Model);
			info.Add ("manufacture", Android.OS.Build.Manufacturer);
			info.Add ("product", Android.OS.Build.Product);
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


