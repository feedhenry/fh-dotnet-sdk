using AeroGear.Push;
using Android.App;
using Android.Content;

namespace FHSDK.Services
{
    internal class AndroidStore : ILocalStore
    {
        private readonly ISharedPreferences _pref = Application.Context.GetSharedPreferences("FHSDK", 0);

        public string Read(string key)
        {
            return _pref.GetString(key, null);
        }

        public void Save(string key, string value)
        {
            _pref.Edit().PutString(key, value).Commit();
        }
    }
}