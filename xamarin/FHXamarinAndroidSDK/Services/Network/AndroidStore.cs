using System;
using AeroGear.Push;
using Android.Content;
using Android.App;

namespace FHSDK.Services
{
	class AndroidStore : ILocalStore
	{

		ISharedPreferences pref = Application.Context.GetSharedPreferences ("FHSDK", 0);

		public string Read (string key)
		{
			return pref.GetString (key, null);
		}

		public void Save (string key, string value)
		{
			pref.Edit().PutString (key, value).Commit();
		}
	}

}

