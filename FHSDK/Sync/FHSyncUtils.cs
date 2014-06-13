using System;
using Newtonsoft.Json;
using FHSDK.Services;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace FHSDK.Sync
{
	public class FHSyncUtils
	{
		public FHSyncUtils ()
		{
		}

		public static string GenerateSHA1Hash (object pObject)
		{
			JArray sorted = SortObj (pObject);
			string strVal = sorted.ToString (Formatting.None);
			IHashService hasher = ServiceFinder.Resolve<IHashService> ();
			return hasher.GenerateSHA1Hash (strVal);
		}

		public static object Clone (object pData)
		{
			string strVal = SerializeObject (pData);
			object cloned = FHSyncUtils.DeserializeObject (strVal, pData.GetType());
			return cloned;
		}

		public static string SerializeObject(object pData)
		{
			return JsonConvert.SerializeObject (pData);
		}

		public static object DeserializeObject(string pVal, Type t)
		{
			return JsonConvert.DeserializeObject(pVal, t);
		}

		public static string GetDefaultDataDir(string dataId)
		{
			IIOService ioService = ServiceFinder.Resolve<IIOService> ();
			string dirName = "com_feedhenry_sync";
			return Path.Combine (ioService.GetDataPersistDir (), dataId, dirName);
		}

		public static string GetDataFilePath(string dataId, string dataFileName)
		{
			string defaultDataDir = GetDefaultDataDir (dataId);
			return Path.Combine (defaultDataDir, dataFileName);
		}

		private static JArray SortObj(object pObject)
		{
			JToken jsonObj = JToken.FromObject (pObject);
			JArray sorted = new JArray ();
			if (jsonObj.Type == JTokenType.Array) {
				JArray casted = jsonObj as JArray;
				for (int i = 0; i < casted.Count; i++) {
					JObject obj = new JObject ();
					obj ["key"] = i + "";
					JToken value = casted [i];
					if (value.Type == JTokenType.Object || value.Type == JTokenType.Array) {
						obj ["value"] = SortObj(value);
					} else {
						obj ["value"] = value;
					}
					sorted.Add (obj);
				}
			} else if (jsonObj.Type == JTokenType.Object) {
				JObject casted = jsonObj as JObject;
				List<String> keys = casted.Properties ().Select (c => c.Name).ToList ();
				keys.Sort ();
				for (int i = 0; i < keys.Count; i++) {
					string key = keys [i];
					if ( pObject is IFHSyncModel && key.Equals ("UID")) {
						continue;
					}
					JObject obj = new JObject ();
					JToken value = casted [key];
					obj ["key"] = key;
					if (value.Type == JTokenType.Object || value.Type == JTokenType.Array) {
						obj ["value"] = SortObj (value);
					} else {
						obj ["value"] = value;
					}
					sorted.Add (obj);
				}
			} else {
				sorted.Add (jsonObj);
			}
			return sorted;
		}
	}
}

