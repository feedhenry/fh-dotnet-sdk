using System;
using MonoTouch.Foundation;

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

		protected override string doRead(string dataId)
		{
			NSUserDefaults prefs = NSUserDefaults.StandardUserDefaults;
			NSObject value = prefs.ValueForKey (new NSString (dataId));
            if(null != value){
                return ((NSString)value).ToString();
            } else {
                return null;
            }
			
		}

		protected override void doSave(string dataId, string dataValue)
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

