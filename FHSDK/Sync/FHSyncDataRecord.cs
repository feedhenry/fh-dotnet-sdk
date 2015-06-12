using Newtonsoft.Json;

namespace FHSDK.Sync
{
    public class FHSyncDataRecord<T> where T : IFHSyncModel
    {
        private string _hashValue;

        public FHSyncDataRecord()
        {
        }

        public FHSyncDataRecord(T pData)
        {
            Data = (T) FHSyncUtils.Clone(pData);
        }

        [JsonProperty("data")]
        public T Data { private set; get; }

        [JsonProperty("hashvalue")]
        public string HashValue
        {
            get { return _hashValue ?? (_hashValue = FHSyncUtils.GenerateSHA1Hash(Data)); }
        }

        [JsonProperty("uid")]
        public string Uid { get; set; }

        public override string ToString()
        {
            return FHSyncUtils.SerializeObject(this);
        }

        public override bool Equals(object obj)
        {
            var record = obj as FHSyncDataRecord<T>;
            return record != null && HashValue.Equals(record.HashValue);
        }

        public FHSyncDataRecord<T> Clone()
        {
            return (FHSyncDataRecord<T>) FHSyncUtils.Clone(this);
        }

        public static FHSyncDataRecord<T> FromJson(string str)
        {
            return (FHSyncDataRecord<T>) FHSyncUtils.DeserializeObject(str, typeof (FHSyncDataRecord<T>));
        }
    }
}