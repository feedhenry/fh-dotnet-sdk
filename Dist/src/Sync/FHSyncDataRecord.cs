using Newtonsoft.Json;

namespace FHSDK.Sync
{
    /// <summary>
    /// FHSyncDataRecord represents the data record to synchronize.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class FHSyncDataRecord<T> where T : IFHSyncModel
    {
        private string _hashValue;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public FHSyncDataRecord()
        {
        }

        /// <summary>
        /// Contructor with a Data type.
        /// </summary>
        /// <param name="pData"></param>
        public FHSyncDataRecord(T pData)
        {
            Data = (T) FHSyncUtils.Clone(pData);
        }

        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("data")]
        public T Data { private set; get; }

        /// <summary>
        /// Get hash value for this data record.
        /// </summary>
        [JsonProperty("hashvalue")]
        public string HashValue
        {
            get { return _hashValue ?? (_hashValue = FHSyncUtils.GenerateSHA1Hash(Data)); }
        }

        /// <summary>
        /// Get unique identifier for this data record.
        /// </summary>
        [JsonProperty("uid")]
        public string Uid { get; set; }

        /// <summary>
        /// Serialise the data record into a string.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return FHSyncUtils.SerializeObject(this);
        }

        /// <summary>
        /// Is the value of a data record equal to another one.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            var record = obj as FHSyncDataRecord<T>;
            return record != null && HashValue.Equals(record.HashValue);
        }

        /// <summary>
        /// Clone a data record.
        /// </summary>
        /// <returns></returns>
        public FHSyncDataRecord<T> Clone()
        {
            return (FHSyncDataRecord<T>) FHSyncUtils.Clone(this);
        }

        /// <summary>
        /// Build a data record from a JSON object.
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static FHSyncDataRecord<T> FromJson(string str)
        {
            return (FHSyncDataRecord<T>) FHSyncUtils.DeserializeObject(str, typeof (FHSyncDataRecord<T>));
        }
    }
}