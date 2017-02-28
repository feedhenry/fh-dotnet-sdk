using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FHSDK.Services;
using FHSDK.Services.Log;
using FHSDK.Services.Monitor;

namespace FHSDK.Sync
{
    /// <summary>
    ///     The types of notifications that will be emitted by the sync client
    /// </summary>
    public enum SyncNotification
    {
        /// <summary>
        ///     Failed to use the client storage
        /// </summary>
        ClientStorageFailed,

        /// <summary>
        ///     One sync loop has started
        /// </summary>
        SyncStarted,

        /// <summary>
        ///     One sync loop has completed successfully
        /// </summary>
        SyncCompleted,

        /// <summary>
        ///     The device is offline and the changes is only applied locally
        /// </summary>
        OfflineUpdate,

        /// <summary>
        ///     There is collision detected during the sync loop
        /// </summary>
        CollisionDetected,

        /// <summary>
        ///     Local changes have been applied to remote server
        /// </summary>
        RemoteUpdateApplied,

        /// <summary>
        ///     Local changes failed to apply to remote server
        /// </summary>
        RemoteUpdateFailed,

        /// <summary>
        ///     The changes have been applied to local dataset
        /// </summary>
        LocalUpdateApplied,

        /// <summary>
        ///     There are a batch of changes from remote server
        /// </summary>
        DeltaReceived,

        /// <summary>
        ///     There are updates for one record entry from remote server
        /// </summary>
        RecordDeltaReceived,

        /// <summary>
        ///     One sync loop finished with failure
        /// </summary>
        SyncFailed
    };

    /// <summary>
    ///     The event arguments that will be sent to the sync event listeners
    /// </summary>
    public class FHSyncNotificationEventArgs : EventArgs
    {
        /// <summary>
        ///     The id of the dataset
        /// </summary>
        /// <value>The dataset identifier.</value>
        public string DatasetId { set; get; }

        /// <summary>
        ///     The unique universal id of the record
        /// </summary>
        /// <value>The uid.</value>
        public string Uid { private get; set; }

        /// <summary>
        ///     Type fo the notification. See SyncNotification.
        /// </summary>
        /// <value>The code.</value>
        public SyncNotification Code { get; set; }

        /// <summary>
        ///     An message associated with the event argument. Could be empty.
        /// </summary>
        /// <value>The message.</value>
        public string Message { get; set; }

        public override string ToString()
        {
            return string.Format("[FHSyncNotificationEventArgs: DatasetId={0}, Uid={1}, Code={2}, Message={3}]",
                DatasetId, Uid, Code, Message);
        }
    }

    /// <summary>
    ///     The client part of the FH Sync Framework.
    ///     To use the sync framework, you just need to create a data model that implements the IFHSyncModel interface, and let
    ///     the sync client manage that data model for you.
    ///     The sync framework will manage the data model for offline use and sync with the cloud when possible. If a data
    ///     model is managed by the sync framework, you should only use the sync framework
    ///     for any CRUD operations for that model.
    /// </summary>
    public class FHSyncClient
    {
        private const string LogTag = "FHSyncClient";
        private static FHSyncClient _syncClientInstance;
        private static readonly ILogService Logger = ServiceFinder.Resolve<ILogService>();
        private readonly Dictionary<string, object> _datasets = new Dictionary<string, object>();

        private readonly Dictionary<string, DatasetSyncDelegate> _datasetsDelegates =
            new Dictionary<string, DatasetSyncDelegate>();

        private readonly IMonitorService _monitor = ServiceFinder.Resolve<IMonitorService>();
        private readonly int _monitorIntervalInMilliSeconds = 500;
        private FHSyncConfig _globalSyncConfig = new FHSyncConfig();
        private bool _initialised;

        private FHSyncClient()
        {
        }

        /// <summary>
        ///     Notify client storage failed event
        /// </summary>
        public event EventHandler<FHSyncNotificationEventArgs> ClientStorageFailed;

        /// <summary>
        ///     Notify sync loop started event
        /// </summary>
        public event EventHandler<FHSyncNotificationEventArgs> SyncStarted;

        /// <summary>
        ///     Notify sync loop complete event
        /// </summary>
        public event EventHandler<FHSyncNotificationEventArgs> SyncCompleted;

        /// <summary>
        ///     Notify offline update event
        /// </summary>
        public event EventHandler<FHSyncNotificationEventArgs> OfflineUpdate;

        /// <summary>
        ///     Notify collision detected event
        /// </summary>
        public event EventHandler<FHSyncNotificationEventArgs> CollisionDetected;

        /// <summary>
        ///     Notify remote update failed event
        /// </summary>
        public event EventHandler<FHSyncNotificationEventArgs> RemoteUpdateFailed;

        /// <summary>
        ///     Notify local update applied event
        /// </summary>
        public event EventHandler<FHSyncNotificationEventArgs> LocalUpdateApplied;

        /// <summary>
        ///     Notify remote update applied event
        /// </summary>
        public event EventHandler<FHSyncNotificationEventArgs> RemoteUpdateApplied;

        /// <summary>
        ///     Notify delta received event
        /// </summary>
        public event EventHandler<FHSyncNotificationEventArgs> DeltaReceived;

        /// <summary>
        ///     Notify record delta received event
        /// </summary>
        public event EventHandler<FHSyncNotificationEventArgs> RecordDeltaReceived;

        /// <summary>
        ///     Notify sync loop failed event
        /// </summary>
        public event EventHandler<FHSyncNotificationEventArgs> SyncFailed;

        /// <summary>
        ///     Send the event notification
        /// </summary>
        /// <param name="args">FH sync notification event</param>
        protected virtual void OnSyncNotification(FHSyncNotificationEventArgs args)
        {
            Logger.d(LogTag, "Receive Notification : " + args, null);
            EventHandler<FHSyncNotificationEventArgs> handler = null;
            switch (args.Code)
            {
                case SyncNotification.ClientStorageFailed:
                    handler = ClientStorageFailed;
                    break;
                case SyncNotification.CollisionDetected:
                    handler = CollisionDetected;
                    break;
                case SyncNotification.DeltaReceived:
                    handler = DeltaReceived;
                    break;
                case SyncNotification.LocalUpdateApplied:
                    handler = LocalUpdateApplied;
                    break;
                case SyncNotification.OfflineUpdate:
                    handler = OfflineUpdate;
                    break;
                case SyncNotification.RecordDeltaReceived:
                    handler = RecordDeltaReceived;
                    break;
                case SyncNotification.RemoteUpdateApplied:
                    handler = RemoteUpdateApplied;
                    break;
                case SyncNotification.RemoteUpdateFailed:
                    handler = RemoteUpdateFailed;
                    break;
                case SyncNotification.SyncCompleted:
                    handler = SyncCompleted;
                    break;
                case SyncNotification.SyncFailed:
                    handler = SyncFailed;
                    break;
                case SyncNotification.SyncStarted:
                    handler = SyncStarted;
                    break;
                default:
                    break;
            }
            if (null != handler)
            {
                //make sure event handlers won't block current thread
                Task.Run(() => { handler(this, args); });
            }
        }

        /// <summary>
        ///     Get the singleton instance of the FHSyncClient
        /// </summary>
        /// <returns>The instance.</returns>
        public static FHSyncClient GetInstance()
        {
            if (null == _syncClientInstance)
            {
                _syncClientInstance = new FHSyncClient();
            }
            return _syncClientInstance;
        }

        /// <summary>
        ///     Set the global FHSyncConfig for all the datasets.
        ///     This will be used for all the dataset if no instance of FHSyncConfig is provided when managing a sync data model.
        /// </summary>
        /// <param name="syncConfig">Sync config.</param>
        public void Initialise(FHSyncConfig syncConfig)
        {
            if (null != syncConfig)
            {
                _globalSyncConfig = syncConfig;
            }

            if (!_initialised)
            {
                MonitorTask();
            }
            _initialised = true;
        }

        /// <summary>
        ///     Manage the specified sync data model that implements the IFHSyncModel.
        /// </summary>
        /// <param name="datasetId">
        ///     Dataset identifier. The datasetId needs to be unique for your app and will be used to name the
        ///     database collection in the cloud.
        /// </param>
        /// <param name="syncConfig">Sync config. If this is null, the global syncConfig will be used.</param>
        /// <param name="qp">A query parameter that will be passed to the cloud when initialise the dataset.</param>
        /// <typeparam name="T"> It should be a type that implements IFHSyncModel.</typeparam>
        public FHSyncDataset<T> Manage<T>(string datasetId, FHSyncConfig syncConfig, IDictionary<string, string> qp)
            where T : IFHSyncModel
        {
            FHSyncDataset<T> dataset;
            if (!_datasets.ContainsKey(datasetId))
            {
                dataset = FHSyncDataset<T>.Build<T>(datasetId,
                    null == syncConfig ? _globalSyncConfig.Clone() : syncConfig.Clone(), qp, null);
                dataset.SyncNotificationHandler += (sender, e) => { OnSyncNotification(e); };
                _datasets[datasetId] = dataset;
                _datasetsDelegates[datasetId] = dataset.RunSyncLoop;
            }
            else
            {
                dataset = (FHSyncDataset<T>) _datasets[datasetId];
                if (null != syncConfig)
                {
                    dataset.SyncConfig = syncConfig;
                }
                if (null != qp)
                {
                    dataset.QueryParams = qp;
                }
            }

            return dataset;
        }

        /// <summary>
        ///     List the data records for the specified datasetId.
        /// </summary>
        /// <param name="datasetId">Dataset identifier.</param>
        /// <typeparam name="T">It should be a type that implements IFHSyncModel.</typeparam>
        public List<T> List<T>(string datasetId) where T : IFHSyncModel
        {
            if (_datasets.ContainsKey(datasetId))
            {
                var dataset = (FHSyncDataset<T>) _datasets[datasetId];
                return dataset.List();
            }
            return null;
        }

        /// <summary>
        ///     Read the data records with the specified datasetId and uid.
        /// </summary>
        /// <param name="datasetId">Dataset identifier.</param>
        /// <param name="uid">The unique id of the data model</param>
        /// <typeparam name="T">It should be a type that implements IFHSyncModel.</typeparam>
        public T Read<T>(string datasetId, string uid) where T : IFHSyncModel
        {
            if (_datasets.ContainsKey(datasetId))
            {
                var dataset = (FHSyncDataset<T>) _datasets[datasetId];
                return dataset.Read(uid);
            }
            return default(T);
        }

        /// <summary>
        ///     Create a new data record with the specified datasetId and an instance of the model.
        ///     The new data record will be synced to the cloud automatically.
        /// </summary>
        /// <param name="datasetId">Dataset identifier.</param>
        /// <param name="model">An instance of the data model T.</param>
        /// <typeparam name="T">It should be a type that implements IFHSyncModel.</typeparam>
        public T Create<T>(string datasetId, T model) where T : IFHSyncModel
        {
            if (_datasets.ContainsKey(datasetId))
            {
                var dataset = (FHSyncDataset<T>) _datasets[datasetId];
                return dataset.Create(model);
            }
            return default(T);
        }

        /// <summary>
        ///     Update the data record with the specified datasetId and model.
        ///     The changes of the data record will be synced to the cloud automatically.
        ///     In case of collision, the collision will be recorded and the local change will be reverted to match the cloud
        ///     entry.
        /// </summary>
        /// <param name="datasetId">Dataset identifier.</param>
        /// <param name="model">An instance of the data model T.</param>
        /// <typeparam name="T">It should be a type that implements IFHSyncModel.</typeparam>
        public T Update<T>(string datasetId, T model) where T : IFHSyncModel
        {
            if (_datasets.ContainsKey(datasetId))
            {
                var dataset = (FHSyncDataset<T>) _datasets[datasetId];
                return dataset.Update(model);
            }
            return default(T);
        }

        /// <summary>
        ///     Delete the data record with the specified datasetId and model.
        ///     The deletion will be applied to local data immediately and sync with cloud when possible.
        ///     In case of collision, the collision will be recorded and the local change will be reverted to match the cloud
        ///     entry.
        /// </summary>
        /// <param name="datasetId">Dataset identifier.</param>
        /// <param name="uid">The uid of the record to delete</param>
        /// <typeparam name="T">It should be a type that implements IFHSyncModel.</typeparam>
        public T Delete<T>(string datasetId, string uid) where T : IFHSyncModel
        {
            if (_datasets.ContainsKey(datasetId))
            {
                var dataset = (FHSyncDataset<T>) _datasets[datasetId];
                return dataset.Delete(uid);
            }
            return default(T);
        }

        /// <summary>
        ///     Stop syncing the specified dataset with the cloud. All the changes will be saved locally only.
        /// </summary>
        /// <param name="datasetId">Dataset identifier.</param>
        /// <typeparam name="T">It should be a type that implements IFHSyncModel.</typeparam>
        public void Stop<T>(string datasetId) where T : IFHSyncModel
        {
            if (_datasets.ContainsKey(datasetId))
            {
                var dataset = (FHSyncDataset<T>) _datasets[datasetId];
                dataset.StopSync();
            }
        }

        /// <summary>
        ///     Start syncing the specified dataset with the cloud if possible
        /// </summary>
        /// <param name="datasetId">Dataset identifier.</param>
        /// <typeparam name="T">It should be a type that implements IFHSyncModel.</typeparam>
        public void Start<T>(string datasetId) where T : IFHSyncModel
        {
            if (_datasets.ContainsKey(datasetId))
            {
                var dataset = (FHSyncDataset<T>) _datasets[datasetId];
                dataset.StartSync();
            }
        }

        /// <summary>
        ///     Invoke a sync loop almost immediately.
        ///     It will guarantee a sync loop will run in the next 500 milliseconds (even the data model has set to stop sync - but
        ///     not if StopAll is called.).
        /// </summary>
        /// <param name="datasetId">Dataset identifier.</param>
        /// <typeparam name="T">It should be a type that implements IFHSyncModel.</typeparam>
        public void ForceSync<T>(string datasetId) where T : IFHSyncModel
        {
            if (_datasets.ContainsKey(datasetId))
            {
                var dataset = (FHSyncDataset<T>) _datasets[datasetId];
                dataset.ForceSync = true;
            }
        }

        /// <summary>
        ///     Stop syncing all local data models.
        /// </summary>
        public void StopAll()
        {
            _monitor.StopMonitor();
        }

        /// <summary>
        ///     Start syncing all local data models.
        /// </summary>
        public void StartAll()
        {
            MonitorTask();
        }

        private void CheckDatasets()
        {
            foreach (var d in _datasetsDelegates.Values)
            {
                d();
            }
        }

        private void MonitorTask()
        {
            if (!_monitor.IsRunning)
            {
                _monitor.MonitorInterval = _monitorIntervalInMilliSeconds;
                CheckDatasetDelegate d = CheckDatasets;
                _monitor.StartMonitor(d);
            }
        }

        private delegate void DatasetSyncDelegate();
    }
}