using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace FHSDK.Sync
{
    /// <summary>
    /// FHSyncPendingRecord represents the data record when they are pending for synchronization.
    /// </summary>
    /// <typeparam name="T"></typeparam>
	public class FHSyncPendingRecord<T> where T : IFHSyncModel
	{
        /// <summary>
        /// Is the record in-flight ie: sent to the wired.
        /// </summary>
        [JsonProperty("inFlight")]
        public Boolean InFlight { set; get; }

        /// <summary>
        /// When was the record marked inflight.
        /// </summary>
        [JsonProperty("inFlightDate")]
        public DateTime InFlightDate { set; get; }

        /// <summary>
        /// is the record marked as crashed ie: recovery after a cloud app crash.
        /// </summary>
        [JsonProperty("crashed")]
        public Boolean Crashed { set; get; }

        /// <summary>
        /// What kind of action.
        /// </summary>
        [JsonProperty("action")]
        public String Action { set; get; }

        /// <summary>
        /// Unique identifier of the pending data record. In case of a create a temporary uid is assigned until the cloud server side is returned.
        /// </summary>
        [JsonProperty("uid")]
        public string Uid { set; get; }

        /// <summary>
        /// Timestamp is used to create temporary uid.
        /// </summary>
        [JsonProperty("timestamp")]
        public DateTime Timestamp { set; get; }

        /// <summary>
        /// Content hash of the value of pending data record prior its modification.
        /// </summary>
        [JsonProperty("pre")]
        public FHSyncDataRecord<T> PreData { set; get; }

        /// <summary>
        /// Content hash of the value of pending data record after its modification.
        /// </summary>
        [JsonProperty("post")]
        public FHSyncDataRecord<T> PostData { set; get; }

        /// <summary>
        /// How many re-send trial after a crahs has been sent.
        /// </summary>
        [JsonProperty("crashCount")]
        public int CrashedCount { set; get; }

        /// <summary>
        /// Is this pending record marked as delayed ie: we know cloud app is not responding, no need to send a syn request.
        /// </summary>
        [JsonProperty("delayed")]
        public Boolean Delayed { set; get; }

        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("waiting")]
        public string Waiting { set; get; }

        /// <summary>
        /// Hash value of the current pending record.
        /// </summary>
        [JsonProperty("hash")]
        public string Hash { 
            get {
                return this.GetHashValue();
            }
        }

		private String hashValue = null;

        /// <summary>
        /// Get the hash value.
        /// </summary>
        /// <returns></returns>
		public String GetHashValue()
		{
			if(null == hashValue){
				//keep it consistant with ios/android/js
                JObject json = AsJObject();
				hashValue = FHSyncUtils.GenerateSHA1Hash (json);
			}
			return hashValue;
		}
		/// <summary>
        /// Constructor.
        /// </summary>
                	
		public FHSyncPendingRecord ()
		{
			this.Timestamp = DateTime.Now;
		}

        /// <summary>
        /// Build a pending data record from a JSON object.
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
		public static FHSyncPendingRecord<T> FromJSON(string val)
		{
			return (FHSyncPendingRecord<T>) FHSyncUtils.DeserializeObject (val, typeof(FHSyncPendingRecord<T>));
		}

        /// <summary>
        /// Serialize a pending data record to a string representation.
        /// </summary>
        /// <returns></returns>
		public override string ToString ()
		{
			return FHSyncUtils.SerializeObject (this);
		}

        /// <summary>
        /// Is thos onject equal to another.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
		public override bool Equals (object obj)
		{
			if (obj is FHSyncPendingRecord<T>) {
				FHSyncPendingRecord<T> that = obj as FHSyncPendingRecord<T>;
				if (that.GetHashValue().Equals (this.GetHashValue())) {
					return true;
				}
				return false;
			} 
			return false;
		}

        /// <summary>
        /// Increment crash sent trial.
        /// </summary>
		public void IncrementCrashCount()
		{
			this.CrashedCount += 1;
		}

        /// <summary>
        /// Reset chrahs status once cloud app is responsing.
        /// </summary>
        public void ResetCrashStatus()
        {
            this.Crashed = false;
            this.InFlight = false;
            this.CrashedCount = 0;
        }

        /// <summary>
        /// Mark a pending record as delayed.
        /// </summary>
        /// <param name="waitingHash"></param>
        public void SetDelayed(string waitingHash)
        {
            this.Delayed = true;
            this.Waiting = waitingHash;
        }

        /// <summary>
        /// Reset a delayed status for a pending data record.
        /// </summary>
        public void ResetDelayed(){
            this.Delayed = false;
            this.Waiting = null;
        }

        /// <summary>
        /// Serialiaze into JSON.
        /// </summary>
        /// <returns></returns>
        public JObject AsJObject()
        {
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
            return json;
        }
        /// <summary>
        /// Serialize only the hash value of the object.
        /// </summary>
        /// <returns></returns>
        public JObject AsJObjectWithHash()
        {
            JObject json = AsJObject();
            json["hash"] = this.hashValue;
            return json;
        }

	}
}

