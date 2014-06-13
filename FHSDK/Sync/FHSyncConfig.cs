using System;

namespace FHSDK.Sync
{
	public class FHSyncConfig
	{
        public enum SyncCloudType {
            AUTO,
            MBBAS,
            LEGACY
        }

		/// <summary>
		/// How often the sync loop should be running. In seconds
		/// </summary>
		/// <value>The sync frequency.</value>
		public int SyncFrequency { set; get; }

		/// <summary>
		/// If there is updates to the local data, should the sync loop be invoked immediately
		/// </summary>
		/// <value><c>true</c> if auto sync local updates; otherwise, <c>false</c>.</value>
		public Boolean AutoSyncLocalUpdates { set; get; }

		/// <summary>
		/// If a record is crashed during a sync loop, how many loops should it be waiting until try again
		/// </summary>
		/// <value>The crashed count wait.</value>
		public int CrashedCountWait { set; get; }

		/// <summary>
		/// If a record is crashed during a sync loop, should it be resent in the future
		/// </summary>
		/// <value><c>true</c> if resend crashed updated; otherwise, <c>false</c>.</value>
		public Boolean ResendCrashedUpdated { set; get; }

		/// <summary>
		/// Control if the sync should ba activated
		/// </summary>
		/// <value><c>true</c> if sync active; otherwise, <c>false</c>.</value>
		public Boolean SyncActive { set; get; }

		/// <summary>
		/// Control if the sync client should be calling the FH MBAAS sync endpoint or the legacy (custom) sync endpoint
		/// </summary>
		/// <value><c>true</c> if use custom sync; otherwise, <c>false</c>.</value>
        public SyncCloudType SyncCloud { set; get; }

		/// <summary>
		/// Specify where the data files should be persisited. If not provided, a default path will be provided
		/// </summary>
		/// <value>The data persistance dir.</value>
		public string DataPersistanceDir { set; get; }

		public FHSyncConfig ()
		{
			this.SyncFrequency = 10;
			this.AutoSyncLocalUpdates = true;
			this.CrashedCountWait = 10;
			this.ResendCrashedUpdated = true;
			this.SyncActive = true;
			this.SyncCloud = SyncCloudType.AUTO;
		}

		public static FHSyncConfig FromJSON(string jsonStr)
		{
			return (FHSyncConfig) FHSyncUtils.DeserializeObject (jsonStr, typeof(FHSyncConfig));
		}

		public override string ToString ()
		{
			return FHSyncUtils.SerializeObject (this);
		}

		public FHSyncConfig Clone()
		{
			return (FHSyncConfig) FHSyncUtils.Clone (this);
		}
	}
}

