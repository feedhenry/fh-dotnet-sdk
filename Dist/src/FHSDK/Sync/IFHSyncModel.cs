namespace FHSDK.Sync
{
    /// <summary>
    ///     An instance to indicate that a data model can be used by the sync client.
    ///     To allow the FHSyncClient to manage data syncing, the data models have to implement this interface.
    /// </summary>
    public interface IFHSyncModel
    {
        /// <summary>
        ///     The unique universal id of the record.
        ///     The implementation of the property should be public readable and writable, and non-serializable (e.g. use
        ///     JsonIgnore attribute or NonSerializedAttribute).
        ///     You should not set the value of the property in your code. The FHSyncClient will set the value for you.
        /// </summary>
        /// <value></value>
        string UID { set; get; }
    }
}