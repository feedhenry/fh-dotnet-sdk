using System;
using FHSDK.Services.Data;
using Foundation;

namespace FHSDK.Services
{
    /// <summary>
    /// Data Service provider for ios
    /// </summary>
	public class DataService: DataServiceBase
	{
		public DataService () : base()
		{
		}

		protected override string DoRead(string dataId)
		{
			NSUserDefaults prefs = NSUserDefaults.StandardUserDefaults;
			NSObject value = prefs.ValueForKey (new NSString (dataId));
            if(null != value){
                return ((NSString)value).ToString();
            } else {
                return null;
            }
			
		}

		protected override void DoSave(string dataId, string dataValue)
		{
			NSUserDefaults prefs = NSUserDefaults.StandardUserDefaults;
			prefs.SetValueForKey (new NSString (dataValue), new NSString (dataId));
			prefs.Synchronize ();
		}

		public override void DeleteData(string dataId)
        {
            NSUserDefaults prefs = NSUserDefaults.StandardUserDefaults;
            prefs.RemoveObject(new NSString(dataId));
            prefs.Synchronize();
        }
	}
}

