using FHSDK.Sync;
using Newtonsoft.Json;

namespace FHSDKTestShared
{
    public class TaskModel : IFHSyncModel
    {
        [JsonProperty("taskName")]
        public string TaksName { set; get; }

        [JsonProperty("completed")]
        public bool Completed { set; get; }

        [JsonIgnore]
        public string UID { set; get; }

        public override string ToString()
        {
            return string.Format("[TaskModel: UID={0}, TaksName={1}, Completed={2}]", UID, TaksName, Completed);
        }
    }
}