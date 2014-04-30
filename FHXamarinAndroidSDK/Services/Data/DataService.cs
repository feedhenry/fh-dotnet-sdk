using System;
using Android.App;
using FHSDK.Services;

namespace FHSDK.Services
{
	public class DataService: DataServiceBase
	{
		private const String PREF_ID = "fhprefs";
		public DataService () : base()
		{
		}

		protected override string doRead(string dataId)
		{
			var prefs = Application.Context.GetSharedPreferences (PREF_ID, Android.Content.FileCreationMode.Private);
			var value = prefs.GetString (dataId, null);
			return value;
		}

		protected override void doSave(string dataId, string dataValue)
		{
			var prefs = Application.Context.GetSharedPreferences (PREF_ID, Android.Content.FileCreationMode.Private);
			var prefsEditor = prefs.Edit ();
			prefsEditor.PutString(dataId, dataValue);
			prefsEditor.Commit();
		}
	}
}

