namespace FHSDK.Services.Data
{
    /// <summary>
    ///     A service inferface that provides key/value pair data saving and retriving.
    /// </summary>
    public interface IDataService
    {
        /// <summary>
        ///     Save the data value associated with the data id
        /// </summary>
        /// <param name="dataId">the key</param>
        /// <param name="dataValue">the value</param>
        void SaveData(string dataId, string dataValue);

        /// <summary>
        ///     Retrieve the data value associated with the data id
        /// </summary>
        /// <param name="dataId">key</param>
        /// <returns>value</returns>
        string GetData(string dataId);

        /// <summary>
        ///     Remove the data value associated with the data id
        /// </summary>
        /// <param name="dataId"></param>
        void DeleteData(string dataId);
    }
}