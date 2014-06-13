using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;

namespace FHSDK.Sync
{
	public class FHSyncPendingRecord
	{
		[JsonProperty("inFlight")]
		public Boolean InFlight { set; get; }

		[JsonProperty("inFlightDate")]
		public DateTime InFlightDate { set; get; }

		[JsonProperty("crashed")]
		public Boolean Crashed { set; get; }

		[JsonProperty("action")]
		public String Action { set; get; }

		[JsonProperty("uid")]
		public string Uid { set; get; }

		[JsonProperty("timestamp")]
		[JsonConverter(typeof(JavaScriptDateTimeConverter))]
		public DateTime Timestamp { set; get; }

		[JsonProperty("preData")]
		public FHSyncDataRecord PreData { set; get; }

		[JsonProperty("postData")]
		public FHSyncDataRecord PostData { set; get; }

		[JsonProperty("crashCount")]
		public int CrashedCount { set; get; }

        [JsonProperty("delayed")]
        public Boolean Delayed { set; get; }

        [JsonProperty("waiting")]
        public string Waiting { set; get; }

		private String hashValue = null;

		public String GetHashValue()
		{
			if(null == hashValue){
				//keep it consistant with ios/android/js
				JObject json = new JObject ();
				json ["inFlight"] = this.InFlight;
				json ["crashed"] = this.Crashed;
				json ["timestamp"] = this.Timestamp;
				json ["inFlightDate"] = this.InFlightDate;
				json ["action"] = this.Action;
				json ["uid"] = this.Uid;
				if (null != this.PreData) {
					json ["pre"] = JToken.FromObject(this.PreData.Data);
					json ["preHash"] = this.PreData.HashValue;
				}
				if (null != this.PostData) {
					json ["post"] = JToken.FromObject(this.PostData.Data);
					json ["postHash"] = this.PostData.HashValue;
				}
				hashValue = FHSyncUtils.GenerateSHA1Hash (json);
			}
			return hashValue;
		}
			
		public FHSyncPendingRecord ()
		{
			this.Timestamp = DateTime.Now;
		}

		public static FHSyncPendingRecord FromJSON(string val)
		{
			return (FHSyncPendingRecord) FHSyncUtils.DeserializeObject (val, typeof(FHSyncPendingRecord));
		}

		public override string ToString ()
		{
			return FHSyncUtils.SerializeObject (this);
		}

		public override bool Equals (object obj)
		{
			if (obj is FHSyncPendingRecord) {
				FHSyncPendingRecord that = obj as FHSyncPendingRecord;
				if (that.GetHashValue().Equals (this.GetHashValue())) {
					return true;
				}
				return false;
			} 
			return false;
		}

		public void IncrementCrashCount()
		{
			this.CrashedCount += 1;
		}

        public void ResetCrashStatus()
        {
            this.Crashed = false;
            this.InFlight = false;
            this.CrashedCount = 0;
        }

        public void SetDelayed(string waitingHash)
        {
            this.Delayed = true;
            this.Waiting = waitingHash;
        }

        public void ResetDelayed(){
            this.Delayed = false;
            this.Waiting = null;
        }

	}
}

