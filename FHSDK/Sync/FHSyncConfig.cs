namespace FHSDK.Sync
{
    /// <summary>
    /// The configuration options for syncing.
    /// </summary>
    public class FHSyncConfig
    {
        /// <summary>
        /// The type of the cloud part of the sync framework.
        /// </summary>
        public enum SyncCloudType
        {
            /// <summary>
            ///     Check automatically
            /// </summary>
            Auto,

            /// <summary>
            ///     Use the FH MBAAS sync service
            /// </summary>
            Mbbas,

            /// <summary>
            ///     Use the legacy sync service
            /// </summary>
            Legacy
        }

        /// <summary>
        /// Defualt constructor.
        /// </summary>
        public FHSyncConfig()
        {
            SyncFrequency = 10;
            AutoSyncLocalUpdates = true;
            CrashedCountWait = 10;
            ResendCrashedUpdated = true;
            SyncActive = true;
            SyncCloud = SyncCloudType.Auto;
        }

        /// <summary>
        /// How often the sync loop should be running. In seconds.
        /// </summary>
        /// <value>The sync frequency.</value>
        public int SyncFrequency { set; get; }

        /// <summary>
        /// If there is updates to the local data, should the sync loop be invoked immediately.
        /// </summary>
        /// <value><c>true</c> if auto sync local updates; otherwise, <c>false</c>.</value>
        public bool AutoSyncLocalUpdates { set; get; }

        /// <summary>
        /// If a record is crashed during a sync loop, how many loops should it be waiting until try again.
        /// </summary>
        /// <value>The crashed count wait.</value>
        public int CrashedCountWait { set; get; }

        /// <summary>
        /// If a record is crashed during a sync loop, should it be resent in the future.
        /// </summary>
        /// <value><c>true</c> if resend crashed updated; otherwise, <c>false</c>.</value>
        public bool ResendCrashedUpdated { set; get; }

        /// <summary>
        /// Control if the sync should ba activated.
        /// </summary>
        /// <value><c>true</c> if sync active; otherwise, <c>false</c>.</value>
        public bool SyncActive { set; get; }

        /// <summary>
        /// Control if the sync client should be calling the FH MBAAS sync endpoint or the legacy (custom) sync endpoint.
        /// </summary>
        /// <value><c>true</c> if use custom sync; otherwise, <c>false</c>.</value>
        public SyncCloudType SyncCloud { set; get; }

        /// <summary>
        /// Specify where the data files should be persisited. If not provided, a default path will be provided.
        /// </summary>
        /// <value>The data persistance dir.</value>
        public string DataPersistanceDir { set; get; }

        /// <summary>
        /// Build config from json format.
        /// </summary>
        /// <param name="jsonStr"></param>
        /// <returns></returns>
        public static FHSyncConfig FromJson(string jsonStr)
        {
            return (FHSyncConfig) FHSyncUtils.DeserializeObject(jsonStr, typeof (FHSyncConfig));
        }

        /// <summary>
        /// Serialize config to string.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return FHSyncUtils.SerializeObject(this);
        }

        /// <summary>
        /// Clone config.
        /// </summary>
        /// <returns></returns>
        public FHSyncConfig Clone()
        {
            return (FHSyncConfig) FHSyncUtils.Clone(this);
        }
    }
}