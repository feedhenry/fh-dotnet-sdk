using System;
using FHSDK.Sync;
using Newtonsoft.Json;

namespace FHSDKTestShared
{
    public class TaskModel: IFHSyncModel
    {
        public TaskModel()
        {
        }

        [JsonIgnore]
        public String UID { set; get; }


        [JsonProperty("taskName")]
        public String TaksName { set; get; }


        [JsonProperty("completed")]
        public bool Completed { set ; get; }

        public override string ToString()
        {
            return string.Format("[TaskModel: UID={0}, TaksName={1}, Completed={2}]", UID, TaksName, Completed);
        }
    }
}

