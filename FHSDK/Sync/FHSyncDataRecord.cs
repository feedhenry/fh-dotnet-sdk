using System;
using Newtonsoft.Json;

namespace FHSDK.Sync
{
	public class FHSyncDataRecord
	{
		private string hashValue = null;

		[JsonProperty("data")]
		public IFHSyncModel Data { set; get; }

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
		public string Uid {
			get {
				if (this.Data != null) {
					return this.Data.UID;
				}
				return null;

			}
			set {
				if (this.Data != null) {
					this.Data.UID = value;
				}
			}
		}

		public FHSyncDataRecord ()
		{
		}

		public FHSyncDataRecord (IFHSyncModel pData)
		{
			this.Data = (IFHSyncModel) FHSyncUtils.Clone (pData);
		}

		public override string ToString ()
		{
			return FHSyncUtils.SerializeObject (this);
		}

		public override bool Equals (object obj)
		{
			if (obj is FHSyncDataRecord) {
				FHSyncDataRecord that = obj as FHSyncDataRecord;
				if (this.HashValue.Equals (that.HashValue)) {
					return true;
				} 
				return false;
			}
			return false;
		}

		public FHSyncDataRecord Clone()
		{
			return (FHSyncDataRecord) FHSyncUtils.Clone (this);
		}

		public static FHSyncDataRecord FromJSON(string str)
		{
			return (FHSyncDataRecord) FHSyncUtils.DeserializeObject (str, typeof(FHSyncDataRecord));
		}
	}
}

