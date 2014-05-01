using System;
using MonoTouch.Foundation;

namespace FHSDK.Services
{
	public class DataService: DataServiceBase
	{
		public DataService () : base()
		{
		}

		protected override string doRead(string dataId)
		{
			NSUserDefaults prefs = NSUserDefaults.StandardUserDefaults;
			NSObject value = prefs.ValueForKey (new NSString (dataId));
			return ((NSString)value).ToString ();
		}

		protected override void doSave(string dataId, string dataValue)
		{
			NSUserDefaults prefs = NSUserDefaults.StandardUserDefaults;
			prefs.SetValueForKey (new NSString (dataValue), new NSString (dataId));
			prefs.Synchronize ();
		}
	}
}

