using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FHSDK.Services;

namespace FHSDK.Sync
{
    
    public enum SyncNotification
    {
        CLIENT_STORAGE_FAILED,
        SYNC_STARTED,
        SYNC_COMPLETED,
        OFFLINE_UPDATE,
        COLLISION_DETECTED,
        REMOTE_UPDATE_APPLIED,
        REMOTE_UPDATE_FAILED,
        LOCAL_UPDATE_APPLIED,
        DELTA_RECEIVED,
        RECORD_DELTA_RECEIVED,
        SYNC_FAILED
    };

    public class FHSyncNotificationEventArgs : EventArgs
    {
        public string DatasetId { set; get; }

        public string Uid { get; set; }

        public SyncNotification Code { get; set; }

        public string Message { get; set; }

        public override string ToString()
        {
            return string.Format("[FHSyncNotificationEventArgs: DatasetId={0}, Uid={1}, Code={2}, Message={3}]", DatasetId, Uid, Code, Message);
        }
    }
        
    public class FHSyncClient
    {
        private static FHSyncClient syncClientInstance;
        private const string LOG_TAG = "FHSyncClient";

        private int monitorIntervalInMilliSeconds = 500;

        private IMonitorService monitor = ServiceFinder.Resolve<IMonitorService>();

        private bool initialised = false;
        private FHSyncConfig globalSyncConfig = new FHSyncConfig();

        private Dictionary<string, FHSyncDataset<IFHSyncModel>> datasets = new Dictionary<string, FHSyncDataset<IFHSyncModel>>();

        private static ILogService logger = ServiceFinder.Resolve<ILogService>();

        private delegate void MonitorDelegate();


        /// <summary>
        /// Notify client storage failed event
        /// </summary>
        public event EventHandler<FHSyncNotificationEventArgs> ClientStorageFailed;
        /// <summary>
        /// Notify sync loop started event
        /// </summary>
        public event EventHandler<FHSyncNotificationEventArgs> SyncStarted;
        /// <summary>
        /// Notify sync loop complete event
        /// </summary>
        public event EventHandler<FHSyncNotificationEventArgs> SyncCompleted;
        /// <summary>
        /// Notify offline update event
        /// </summary>
        public event EventHandler<FHSyncNotificationEventArgs> OfflineUpdate;
        /// <summary>
        /// Notify collision detected event
        /// </summary>
        public event EventHandler<FHSyncNotificationEventArgs> CollisionDetected;
        /// <summary>
        /// Notify remote update failed event
        /// </summary>
        public event EventHandler<FHSyncNotificationEventArgs> RemoteUpdateFailed;
        /// <summary>
        /// Notify local update applied event
        /// </summary>
        public event EventHandler<FHSyncNotificationEventArgs> LocalUpdateApplied;
        /// <summary>
        /// Notify remote update applied event
        /// </summary>
        public event EventHandler<FHSyncNotificationEventArgs> RemoteUpdateApplied;
        /// <summary>
        /// Notify delta received event
        /// </summary>
        public event EventHandler<FHSyncNotificationEventArgs> DeltaReceived;
        /// <summary>
        /// Notify record delta received event
        /// </summary>
        public event EventHandler<FHSyncNotificationEventArgs> RecordDeltaReceived;
        /// <summary>
        /// Notify sync loop failed event
        /// </summary>
        public event EventHandler<FHSyncNotificationEventArgs> SyncFailed;

        private FHSyncClient()
        {

        }

        /// <summary>
        /// Send the event notification
        /// </summary>
        /// <param name="datasetId">Dataset identifier.</param>
        /// <param name="uid">Uid.</param>
        /// <param name="code">Code.</param>
        /// <param name="message">Message.</param>
        protected virtual void OnSyncNotification(FHSyncNotificationEventArgs args)
        {
            logger.d(LOG_TAG, "Receive Notification : " + args.ToString(), null);
            EventHandler<FHSyncNotificationEventArgs> handler = null;
            switch (args.Code)
            {
                case SyncNotification.CLIENT_STORAGE_FAILED:
                    handler = ClientStorageFailed;
                    break;
                case SyncNotification.COLLISION_DETECTED:
                    handler = CollisionDetected;
                    break;
                case SyncNotification.DELTA_RECEIVED:
                    handler = DeltaReceived;
                    break;
                case SyncNotification.LOCAL_UPDATE_APPLIED:
                    handler = LocalUpdateApplied;
                    break;
                case SyncNotification.OFFLINE_UPDATE:
                    handler = OfflineUpdate;
                    break;
                case SyncNotification.RECORD_DELTA_RECEIVED:
                    handler = RecordDeltaReceived;
                    break;
                case SyncNotification.REMOTE_UPDATE_APPLIED:
                    handler = RemoteUpdateApplied;
                    break;
                case SyncNotification.REMOTE_UPDATE_FAILED:
                    handler = RemoteUpdateFailed;
                    break;
                case SyncNotification.SYNC_COMPLETED:
                    handler = SyncCompleted;
                    break;
                case SyncNotification.SYNC_FAILED:
                    handler = SyncFailed;
                    break;
                case SyncNotification.SYNC_STARTED:
                    handler = SyncStarted;
                    break;
                default:
                    break;
            }
            if (null != handler)
            {
                //make sure event handlers won't block current thread
                Task.Run(() =>
                {
                    handler(this, args);
                });
            }
        }


        public static FHSyncClient GetInstance()
        {
            if(null == syncClientInstance){
                syncClientInstance = new FHSyncClient();
            }
            return syncClientInstance;
        }

        public void Initialise(FHSyncConfig syncConfig)
        {
          if(null != syncConfig){
                this.globalSyncConfig = syncConfig;
          }

          if(!initialised){
            this.MonitorTask();
          }
          this.initialised = true;
        }

        public FHSyncDataset<IFHSyncModel> Manage(string datasetId, FHSyncConfig syncConfig , IDictionary<string, string> qp)
        {
            FHSyncDataset<IFHSyncModel> dataset = null;
            if(!this.datasets.ContainsKey(datasetId)){
                dataset = FHSyncDataset<IFHSyncModel>.Build<IFHSyncModel>(datasetId, null == syncConfig? globalSyncConfig.Clone() : syncConfig.Clone(), qp, null);
                dataset.SyncNotificationHandler += (sender, e) =>
                {
                    this.OnSyncNotification(e);
                };
                datasets[datasetId] = dataset;
            }else {
                dataset = this.datasets[datasetId];
            }

            return dataset;
        }

        public List<IFHSyncModel> List(string datasetId)
        {
            if(this.datasets.ContainsKey(datasetId)){
                FHSyncDataset<IFHSyncModel> dataset = this.datasets[datasetId];
                return dataset.List();
            } else {
                return null;
            }
        }

        public IFHSyncModel Read(string datasetId, string uid)
        {
            if(this.datasets.ContainsKey(datasetId)){
                FHSyncDataset<IFHSyncModel> dataset = this.datasets[datasetId];
                return dataset.Read(uid);
            } else {
                return null;
            }
        }

        public IFHSyncModel Create(string datasetId, IFHSyncModel model)
        {
            if(this.datasets.ContainsKey(datasetId)){
                FHSyncDataset<IFHSyncModel> dataset = this.datasets[datasetId];
                return dataset.Create(model);
            } else {
                return null;
            }
        }

        public IFHSyncModel Update(string datasetId, IFHSyncModel model)
        {
            if(this.datasets.ContainsKey(datasetId)){
                FHSyncDataset<IFHSyncModel> dataset = this.datasets[datasetId];
                return dataset.Update(model);
            } else {
                return null;
            }
        }

        public IFHSyncModel Delete(string datasetId, string uid)
        {
            if(this.datasets.ContainsKey(datasetId)){
                FHSyncDataset<IFHSyncModel> dataset = this.datasets[datasetId];
                return dataset.Delete(uid);
            } else {
                return null;
            }
        }

        public void Stop(string datasetId)
        {
            if(this.datasets.ContainsKey(datasetId)){
                FHSyncDataset<IFHSyncModel> dataset = this.datasets[datasetId];
                dataset.StopSync();
            }
        }

        public void Start(string datasetId)
        {
            if(this.datasets.ContainsKey(datasetId)){
                FHSyncDataset<IFHSyncModel> dataset = this.datasets[datasetId];
                dataset.StartSync();
            }
        }

        public void StopAll()
        {
            this.monitor.StopMonitor();
        }

        public void StartAll()
        {
            this.MonitorTask();
        }


        private void CheckDatasets()
        {
            foreach(var dataset in datasets.Values){
                if(dataset.ShouldSync()){
                    dataset.StartSyncLoop();
                }
            }
        }

        private void MonitorTask()
        {
           if(!monitor.IsRunning) {
               monitor.MonitorInterval = this.monitorIntervalInMilliSeconds;
               MonitorDelegate d = new MonitorDelegate(CheckDatasets);
               monitor.StartMonitor(d);
           }
        }
    }
}

