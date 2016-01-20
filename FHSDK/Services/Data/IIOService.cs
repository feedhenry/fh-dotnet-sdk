namespace FHSDK.Services.Data
{
    /// <summary>
    /// Interface that holds i/o operation for data storage.
    /// </summary>
    public interface IIOService
    {
        /// <summary>
        /// Read file.
        /// </summary>
        /// <param name="fullPath"></param>
        /// <returns></returns>
        string ReadFile(string fullPath);

        /// <summary>
        /// Write to file.
        /// </summary>
        /// <param name="fullPath"></param>
        /// <param name="content"></param>
        void WriteFile(string fullPath, string content);

        /// <summary>
        /// Does file exist?
        /// </summary>
        /// <param name="fullPath"></param>
        /// <returns></returns>
        bool Exists(string fullPath);

        /// <summary>
        /// Get file storage path.
        /// </summary>
        /// <returns></returns>
        string GetDataPersistDir();
    }
}