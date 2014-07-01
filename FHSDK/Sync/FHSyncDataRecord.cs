using System;
using Newtonsoft.Json;

namespace FHSDK.Sync
{
	public class FHSyncDataRecord<T> where T : IFHSyncModel
	{
		private string hashValue = null;

		[JsonProperty("data")]
		public T Data { set; get; }

		[JsonProperty("hashvalue")]
		public string HashValue
		{
			get {
				if (null == hashValue) {
					hashValue = FHSyncUtils.GenerateSHA1Hash (this.Data);
				}
				return hashValue;
			}
		}

		[JsonProperty("uid")]
		public string Uid
        {
            get;
            set;
		}

		public FHSyncDataRecord ()
		{
		}

		public FHSyncDataRecord (T pData)
		{
			this.Data = (T)FHSyncUtils.Clone (pData);
		}

		public override string ToString ()
		{
			return FHSyncUtils.SerializeObject (this);
		}

		public override bool Equals (object obj)
		{
			if (obj is FHSyncDataRecord<T>) {
				FHSyncDataRecord<T> that = obj as FHSyncDataRecord<T>;
				if (this.HashValue.Equals (that.HashValue)) {
					return true;
				} 
				return false;
			}
			return false;
		}

		public FHSyncDataRecord<T> Clone()
		{
			return (FHSyncDataRecord<T>) FHSyncUtils.Clone (this);
		}

		public static FHSyncDataRecord<T> FromJSON(string str)
		{
			return (FHSyncDataRecord<T>) FHSyncUtils.DeserializeObject (str, typeof(FHSyncDataRecord<T>));
		}
	}
}

