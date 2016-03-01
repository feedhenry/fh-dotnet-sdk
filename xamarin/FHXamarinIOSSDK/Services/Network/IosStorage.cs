using AeroGear.Push;
using Foundation;

namespace FHSDK.Services
{
    public class IosStorage : ILocalStore
    {
        public string Read(string key)
        {
            return NSUserDefaults.StandardUserDefaults.StringForKey(key);
        }

        public void Save(string key, string value)
        {
            NSUserDefaults.StandardUserDefaults.SetString(value, key);
        }
    }
}