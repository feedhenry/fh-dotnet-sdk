using System;
using System.Collections.Generic;
using FHSDK.Services;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics.Contracts;
using System.Xml;
using Newtonsoft.Json;
using System.Net;
using System.Runtime.CompilerServices;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Converters;

namespace FHSDK.Sync
{
    public class FHSyncDataset<T> where T : IFHSyncModel
    {
        private const string LOG_TAG = "FHSyncDataset";
        private const string PERSIST_FILE_NAME = ".sync.json";
        /// <summary>
        /// If the sync loop is running
        /// </summary>
        public Boolean syncRunning { get; set; }
        /// <summary>
        /// <summary>
        /// Is there any pending sync records
        /// </summary>
        public Boolean syncPending { get; set; }
        /// <summary>
        /// The store of pending records
        /// </summary>
        private IDataStore<FHSyncPendingRecord> pendingRecords;
        /// <summary>
        /// The store of data records
        /// </summary>
        private IDataStore<FHSyncDataRecord> dataRecords;
        /// <summary>
        /// Should the sync be stopped
        /// </summary>
        private Boolean stopSync = false;
        private static ILogService logger = ServiceFinder.Resolve<ILogService>();
        private static INetworkService networkService = ServiceFinder.Resolve<INetworkService>();

        protected enum SyncNotification
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
            SYNC_FAILED}

        ;

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

        /// <summary>
        /// The sync configuration
        /// </summary>
        public FHSyncConfig SyncConfig { set; get; }

        /// <summary>
        /// The hash value of the dataset
        /// </summary>
        public String HashValue { get; set; }

        /// <summary>
        /// The id of the data set the sync client is currently managing
        /// </summary>
        protected string DatasetId { get; set; }

        /// <summary>
        /// When the last sync started
        /// </summary>
        private DateTime SyncStart { get; set; }

        /// <summary>
        /// When the last sync ended
        /// </summary>
        private DateTime SyncEnd { get; set; }

        /// <summary>
        /// The query params for the data records. Will be used to send to the cloud when listing initial records.
        /// </summary>
        protected IDictionary<string, string> QueryParams { get; set; }

        /// <summary>
        /// The meta data for the dataset
        /// </summary>
        protected FHSyncMetaData MetaData { get; set; }

        public Boolean StopSync { get; set; }

        /// <summary>
        /// Records change acknowledgements
        /// </summary>
        protected List<FHSyncResponseUpdatesData> Acknowledgements { get; set; }

        public FHSyncDataset()
        {
        }

        /// <summary>
        /// Init a sync dataset with some parameters
        /// </summary>
        /// <param name="datasetId">Dataset identifier.</param>
        /// <param name="syncConfig">Sync config.</param>
        /// <param name="qp">Query parameters that will be send to the cloud when listing dataset</param>
        /// <param name="meta">Meta data that will be send to the cloud when syncing </param>
        /// <typeparam name="X">The 1st type parameter.</typeparam>
        public static FHSyncDataset<X> Build<X>(string datasetId, FHSyncConfig syncConfig, IDictionary<string, string> qp, FHSyncMetaData meta) where X : IFHSyncModel
        {
            //check if there is a dataset model file exists and load it
            string syncClientMeta = FHSyncUtils.GetDataFilePath(datasetId, PERSIST_FILE_NAME);
            FHSyncDataset<X> dataset = LoadExistingDataSet<X>(syncClientMeta, datasetId);
            if (null == dataset)
            {
                //no existing one, create a new one
                dataset = new FHSyncDataset<X>();
                dataset.DatasetId = datasetId;
                dataset.SyncConfig = syncConfig;
                dataset.QueryParams = qp;
                dataset.MetaData = meta;
                dataset.dataRecords = new InMemoryDataStore<FHSyncDataRecord>();
                dataset.pendingRecords = new InMemoryDataStore<FHSyncPendingRecord>();
                //persist the dataset immediately
                dataset.Save();
            }
            return dataset;
        }

        /// <summary>
        /// List data
        /// </summary>
        public List<T> List()
        {
            List<T> results = new List<T>();
            Dictionary<string, FHSyncDataRecord> storedData = this.dataRecords.List();
            foreach (KeyValuePair<string, FHSyncDataRecord> item in storedData)
            {
                FHSyncDataRecord record = item.Value;
                T data = (T)FHSyncUtils.Clone(record.Data);
                results.Add(data);
            }
            return results;
        }

        /// <summary>
        /// Read data specified by uid.
        /// </summary>
        /// <param name="uid">Uid.</param>
        public T Read(string uid)
        {
            Contract.Assert(null != uid, "uid is null");
            FHSyncDataRecord record = this.dataRecords.Get(uid);
            if (null != record)
            {
                T data = (T)FHSyncUtils.Clone(record.Data);
                return data;
            }
            else
            {
                return default(T);
            }
        }

        /// <summary>
        /// Create data
        /// </summary>
        /// <param name="data">Data.</param>
        public T Create(T data)
        {
            Contract.Assert(data.UID == null, "data is not new");
            T ret = default(T);
            FHSyncPendingRecord pendingRecord = AddPendingRecord(data, "create");
            if (null == pendingRecord)
            {
                //for creation, the uid will be the uid of the pending record temporarily
                FHSyncDataRecord record = this.dataRecords.Get(pendingRecord.Uid);
                if (null != record)
                {
                    ret = (T)FHSyncUtils.Clone(record.Data);
                }

            }
            if (ret.Equals(default(T)))
            {
                throw new Exception("create failed");
            }
            else
            {
                return ret;
            }
        }

        /// <summary>
        /// Update the specified data.
        /// </summary>
        /// <param name="data">Data.</param>
        public T Update(T data)
        {
            Contract.Assert(data.UID != null, "data is new");
            FHSyncDataRecord record = this.dataRecords.Get(data.UID);
            Contract.Assert(null != record, "data record with uid " + data.UID + " doesn't exist");
            T ret = default(T);
            FHSyncPendingRecord pendingRecord = AddPendingRecord(data, "update");
            if (null != pendingRecord)
            {
                FHSyncDataRecord updatedRecord = this.dataRecords.Get(data.UID);
                if (null != updatedRecord)
                {
                    ret = (T)FHSyncUtils.Clone(record.Data);
                }
            }
            if (ret.Equals(default(T)))
            {
                throw new Exception("update failed");
            }
            else
            {
                return ret;
            }
        }

        /// <summary>
        /// Delete the specified uid.
        /// </summary>
        /// <param name="uid">Uid.</param>
        public T Delete(string uid)
        {
            Contract.Assert(null != uid, "uid is null");
            FHSyncDataRecord record = this.dataRecords.Get(uid);
            Contract.Assert(null != record, "data record with uid " + uid + " doesn't exist");
            T ret = default(T);
            FHSyncPendingRecord pendingRecord = AddPendingRecord(record.Data, "delete");
            if (null != pendingRecord)
            {
                ret = (T)FHSyncUtils.Clone(record.Data);
            }
            if (ret.Equals(default(T)))
            {
                throw new Exception("delete failed");
            }
            else
            {
                return ret;
            }
        }

        protected FHSyncPendingRecord AddPendingRecord(IFHSyncModel dataRecords, string action)
        {
            if (!networkService.IsOnline())
            {
                this.OnSyncNotification(dataRecords.UID, SyncNotification.OFFLINE_UPDATE, action);
            }
            //create pendingRecord
            FHSyncPendingRecord pendingRecord = new FHSyncPendingRecord();
            pendingRecord.InFlight = false;
            pendingRecord.Action = action;
            FHSyncDataRecord dataRecord = null;
            if (null != dataRecords)
            {
                dataRecord = new FHSyncDataRecord(dataRecords);
                pendingRecord.PostData = dataRecord;
            }
            if ("create".Equals(action))
            {
                pendingRecord.Uid = pendingRecord.PostData.HashValue;
            }
            else
            {
                FHSyncDataRecord existing = this.dataRecords.Get(dataRecords.UID);
                pendingRecord.Uid = dataRecords.UID;
                pendingRecord.PreData = existing.Clone();
            }
            StorePendingRecord(pendingRecord);
            if("delete".Equals(action)){
                this.dataRecords.Delete(dataRecords.UID);
            } else {
                this.dataRecords.Insert(dataRecord.Uid, dataRecord);
                this.MetaData.InsertBoolMetaData(dataRecord.Uid, "fromPending", true);
                this.MetaData.InsertStringMetaData(dataRecord.Uid, "pendingUid", pendingRecord.GetHashValue());
            }
            return pendingRecord;
        }

        //TODO: probably move this to a dedicated PendingRecordsManager
        protected void StorePendingRecord(FHSyncPendingRecord pendingRecord)
        {
            this.pendingRecords.Insert(pendingRecord.GetHashValue(), pendingRecord);
            string previousPendingUID = null;
            FHSyncPendingRecord previousPending = null;
            string uid = pendingRecord.Uid;
            DebugLog("update local dataset for uid " + uid + " - action = " + pendingRecord.Action);
            FHSyncDataRecord existing = dataRecords.Get(uid);
            Boolean fromPending = this.MetaData.GetMetaDataAsBool(uid, "fromPending");
            if ("create".Equals(pendingRecord.Action)) {
                if (null != existing) {
                    DebugLog("data already exists for uid for create :: " + existing.ToString());
                    if (fromPending) {
                        previousPendingUID = this.MetaData.GetMetaDataAsString(uid, "pendingUid");
                        if (null != previousPendingUID) {
                            this.pendingRecords.Delete(previousPendingUID);
                        }
                    }
                }
            }

            if ("update".Equals(pendingRecord.Action)) {
                if (null != existing) {
                    DebugLog("Update an existing pending record for dataset :: " + existing.ToString());
                    previousPendingUID = this.MetaData.GetMetaDataAsString(uid, "pendingUid");
                    if(null != previousPendingUID){
                        this.MetaData.InsertStringMetaData(uid, "previousPendingUid", previousPendingUID);
                        previousPending = this.pendingRecords.Get(previousPendingUID);
                        if(null != previousPending) {
                            if(!previousPending.InFlight) {
                                DebugLog("existing pre-flight pending record =" + previousPending.ToString());
                                previousPending.PostData = pendingRecord.PostData;
                                pendingRecords.Delete(pendingRecord.GetHashValue());
                            } else {
                                DebugLog("existing in-flight pending record = " + previousPending.ToString());
                                pendingRecord.SetDelayed(previousPending.GetHashValue());
                            }
                        }
                    }

                }
            }

            if("delete".Equals(pendingRecord.Action)){
                if(null != existing){
                    if(fromPending){
                        DebugLog("Deleting an existing pending record for dataset :: " + existing.ToString());
                        previousPendingUID = this.MetaData.GetMetaDataAsString(uid, "pendingUid");
                        if(null != previousPendingUID){
                            this.MetaData.InsertStringMetaData(uid, "previousPendingUid", previousPendingUID);
                            previousPending = this.pendingRecords.Get(previousPendingUID);
                            if(!previousPending.InFlight){
                                DebugLog("existing pending record = " + previousPending.ToString());
                                if("create".Equals(previousPending.Action)){
                                    this.pendingRecords.Delete(pendingRecord.GetHashValue());
                                    this.pendingRecords.Delete(previousPendingUID);
                                }
                                if("update".Equals(previousPending.Action)){
                                    pendingRecord.PreData = previousPending.PreData;
                                    pendingRecord.InFlight = false;
                                    this.pendingRecords.Delete(previousPendingUID);
                                }
                            } else {
                                DebugLog("existing in-flight pending record = " + previousPending.ToString());
                                pendingRecord.SetDelayed(previousPending.GetHashValue());
                            }
                        }

                    }
                }
            }

            if(this.SyncConfig.AutoSyncLocalUpdates){
                this.syncPending = true;
            }
            this.Save();
            this.OnSyncNotification(uid, SyncNotification.LOCAL_UPDATE_APPLIED, pendingRecord.Action);
        }

        public async void StartSyncLoop()
        {
            this.syncPending = false;
            this.syncRunning = true;
            this.SyncStart = DateTime.Now;
            this.OnSyncNotification(null, SyncNotification.SYNC_STARTED, null);
            if(networkService.IsOnline()){
                FHSyncLoopParams syncParams = new FHSyncLoopParams(this);
                if(syncParams.Pendings.Count > 0){
                    logger.i(LOG_TAG, "starting sync loop - global hash = " + this.HashValue + " :: params = " + syncParams.ToString(), null);
                    try {
                        FHResponse syncRes = await DoCloudCall(syncParams);
                        if(null == syncRes.Error){
                            FHSyncResponseData returnedSyncData = (FHSyncResponseData)FHSyncUtils.DeserializeObject(syncRes.RawResponse, typeof(FHSyncResponseData));

                            //TODO: it should be possible achieve the same effects using one loop through the pending records, there is no need to loop the pending records 6 times!
                            //e.g. 
                            /**
                             * for each pending in pendingRecords
                             *   check if sync response contains update for the pending
                             *       true => update pending pre data from the syn response
                             *       false => update syn response with the pending record post data
                             *          
                             *   if pending is in flight
                             *     if pending is crashed
                             *       check if there is updates for the crashed record
                             *        true => resole the crash status
                             *        false => keep waiting or give up
                             *   
                             *   if pendingRecord is delayed
                             *     check if sync response contains info about the delay records
                             *       true => resolve delayed status
                             */

                            // Check to see if any new pending records need to be updated to reflect the current state of play.
                            this.UpdatePendingFromNewData(returnedSyncData);

                            // Check to see if any previously crashed inflight records can now be resolved
                            this.UpdateCrashedInFlightFromNewData(returnedSyncData);

                            //Check to see if any delayed pending records can now be set to ready
                            this.UpdateDelayedFromNewData(returnedSyncData);

                            //Check meta data as well to make sure it contains the correct info
                            this.UpdateMetaFromNewData(returnedSyncData);

                            // Update the new dataset with details of any inflight updates which we have not received a response on
                            this.UpdateNewDataFromInFlight(returnedSyncData);

                            // Update the new dataset with details of any pending updates
                            this.UpdateNewDataFromPending(returnedSyncData);

                            if(null != returnedSyncData.Records){
                                this.UpdateLocalDatasetFromRemote(returnedSyncData);
                            }

                            if(null != returnedSyncData.Updates){
                                this.ProcessUpdatesFromRemote(returnedSyncData);
                            }

                            if(null == returnedSyncData.Records && returnedSyncData.Hash != null){
                                DebugLog("Local dataset stale - syncing records :: local hash = " + this.HashValue + " - remoteHash = " + returnedSyncData.Hash);
                                //Different hash value returned - sync individual records
                                this.SyncRecords();
                            } else {
                                DebugLog("Local dataset up to date");
                                this.SyncLoopComplete("online", SyncNotification.SYNC_COMPLETED);
                            }
                        } else {
                            // The HTTP call failed to complete succesfully, so the state of the current pending updates is unknown
                            // Mark them as "crashed". The next time a syncLoop completets successfully, we will review the crashed
                            // records to see if we can determine their current state.
                            this.MarkInFlightAsCrased();
                            DebugLog("syncLoop failed :: res = " + syncRes.RawResponse + " err = " + syncRes.Error);
                            this.SyncLoopComplete(syncRes.RawResponse, SyncNotification.SYNC_FAILED);

                        }
                    } catch (Exception e) {
                        DebugLog("Error performing sync - " + e.ToString());
                        this.SyncLoopComplete(e.Message, SyncNotification.SYNC_FAILED);
                    }     
                }
            } else {
                this.OnSyncNotification(null, SyncNotification.SYNC_FAILED, "offline");
            }
        }

        private async void SyncRecords()
        {
            FHSyncRecordsParams syncParams = new FHSyncRecordsParams(this);
            FHResponse syncRecordsRes = await this.DoCloudCall(syncParams);
            if(null == syncRecordsRes.Error){
                FHSyncRecordsResponseData remoteDataRecords = (FHSyncRecordsResponseData) FHSyncUtils.DeserializeObject(syncRecordsRes.RawResponse, typeof(FHSyncRecordsResponseData));

                IDataStore<FHSyncDataRecord> updatedDatasetRecords = new InMemoryDataStore<FHSyncDataRecord>();
                updatedDatasetRecords.PersistPath = this.dataRecords.PersistPath;

                Dictionary<string, FHSyncDataRecord> createdRecords = remoteDataRecords.CreatedRecords;
                foreach(var created in createdRecords){
                    updatedDatasetRecords.Insert(created.Key, created.Value);
                    this.OnSyncNotification(created.Key, SyncNotification.RECORD_DELTA_RECEIVED, "create");
                }

                Dictionary<string, FHSyncDataRecord> updatedRecords = remoteDataRecords.UpdatedRecords;
                foreach(var updated in updatedRecords){
                    updatedDatasetRecords.Insert(updated.Key, updated.Value);
                    this.OnSyncNotification(updated.Key, SyncNotification.RECORD_DELTA_RECEIVED, "update");
                }

                Dictionary<string, FHSyncDataRecord> deletedRecords = remoteDataRecords.DeletedRecords;
                foreach (var deleted in deletedRecords) {
                    updatedDatasetRecords.Delete(deleted.Key);
                    this.OnSyncNotification(deleted.Key, SyncNotification.RECORD_DELTA_RECEIVED, "delete");    
                }


                this.OnSyncNotification(remoteDataRecords.Hash, SyncNotification.DELTA_RECEIVED, "partial dataset");
                this.dataRecords = updatedDatasetRecords;
                if(null != remoteDataRecords.Hash){
                    this.HashValue = remoteDataRecords.Hash;
                }

                this.SyncLoopComplete("online", SyncNotification.SYNC_COMPLETED);
            } else {
                DebugLog("SyncRecords failed : " + syncRecordsRes.RawResponse + " error = " + syncRecordsRes.Error);
                this.SyncLoopComplete(syncRecordsRes.RawResponse, SyncNotification.SYNC_FAILED);
            }

        }

        private void SyncLoopComplete(string message, SyncNotification notification)
        {
            this.syncRunning = false;
            this.SyncEnd = DateTime.Now;
            this.Save();
            this.OnSyncNotification(this.HashValue, notification, message);
        }

        private void UpdatePendingFromNewData(FHSyncResponseData syncResData)
        {
            if(null != pendingRecords && null != syncResData.Records){
                Dictionary<string, FHSyncPendingRecord> localPendingRecords = pendingRecords.List();
                foreach (var item in localPendingRecords)
                {
                    FHSyncPendingRecord pendingRecord = item.Value;
                    if(!pendingRecord.InFlight){
                        //process pending records that have not been submitted
                        DebugLog("Found Non in flight record -> action = " + pendingRecord.Action + " :: uid=" + pendingRecord.Uid + " :: hash=" + pendingRecord.GetHashValue());
                        if("update".Equals(pendingRecord.Action) || "delete".Equals(pendingRecord.Action)){
                            //update the prevalue of pending record to reflect the latest data returned from sync
                            //This will prevent a collision being reported when the pending record is sent
                            //TODO: is this mean we are blindly apply changes from remote to the current store, then when the local change is submitted, the remote data will be overridden by local updates even local updates could be wrong?
                            FHSyncDataRecord returnedRecord = null;
                            syncResData.Records.TryGetValue(pendingRecord.Uid, out returnedRecord);
                            if(null != returnedRecord){
                                DebugLog("updating pre values for existing pending record " + pendingRecord.Uid);
                                pendingRecord.PreData = returnedRecord;
                            } else {
                                //The update/delete maybe for a newly created record in which case the uid will have changed
                                string previousPendingUid = this.MetaData.GetMetaDataAsString(pendingRecord.Uid, "previousPendingUid");
                                if(null != previousPendingUid){
                                    FHSyncPendingRecord previousPendingRecord = null;
                                    localPendingRecords.TryGetValue(previousPendingUid, out previousPendingRecord);
                                    if(null != previousPendingRecord){
                                        FHSyncResponseUpdatesData appliedRecord = syncResData.GetAppliedUpdates(previousPendingRecord.GetHashValue());
                                        if(null != appliedRecord){
                                            string newUid = appliedRecord.Uid;
                                            FHSyncDataRecord newRecord = syncResData.GetRemoteRecord(newUid);
                                            if(null != newRecord){
                                                DebugLog("Updating pre values for existing pending record which was previously a create " + pendingRecord.Uid + " => " + newUid);
                                                pendingRecord.PreData = newRecord;
                                                pendingRecord.Uid = newUid;
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        if("create".Equals(pendingRecord.Action)){
                            FHSyncResponseUpdatesData appliedRecord = syncResData.GetAppliedUpdates(pendingRecord.GetHashValue());
                            if(null != appliedRecord){
                                DebugLog("Found an update for a pending create + " + appliedRecord.ToString());
                                FHSyncDataRecord newRecord = syncResData.GetRemoteRecord(pendingRecord.GetHashValue());
                                if(null != newRecord){
                                    DebugLog("Changing pending create to an update based on new record " + newRecord.ToString());
                                    pendingRecord.Action = "update";
                                    pendingRecord.PreData = newRecord;
                                    pendingRecord.Uid = appliedRecord.Uid;
                                }
                            }
                        }
                    }
                }
            }
        }

        private void UpdateCrashedInFlightFromNewData(FHSyncResponseData syncResData)
        {
            Dictionary<string, FHSyncPendingRecord> localPendingRecords = this.pendingRecords.List();

            foreach (string pendingRecordKey in localPendingRecords.Keys)
            {
                bool processed = false;
                FHSyncPendingRecord pendingRecord = localPendingRecords[pendingRecordKey];
                if(pendingRecord.InFlight && pendingRecord.Crashed){
                    DebugLog("Found crashed inFlight pending record uid =" + pendingRecord.Uid + " :: hash = " + pendingRecord.GetHashValue());
                    if(null != syncResData.Updates && syncResData.Updates.ContainsKey("hashes") ){
                        FHSyncResponseUpdatesData crashedUpdate = syncResData.GetUpdateByHash(pendingRecord.GetHashValue());
                        if(null != crashedUpdate){
                            DebugLog("resolving status for crashed inflight pending record " + crashedUpdate.ToString());
                            if(crashedUpdate.Type == FHSyncResponseUpdatesData.FHSyncResponseUpdatesDataType.failed){
                                if(crashedUpdate.Action.Equals("create")){
                                    DebugLog("Deleting failed create from dataset");
                                    this.dataRecords.Delete(crashedUpdate.Uid);
                                } else if(crashedUpdate.Action.Equals("update") || crashedUpdate.Action.Equals("delete")){
                                    DebugLog("Reverting failed " + crashedUpdate.Action + " in dataset");
                                    this.dataRecords.Insert(crashedUpdate.Uid, pendingRecord.PreData);
                                }
                            }

                            this.pendingRecords.Delete(pendingRecordKey);
                            switch (crashedUpdate.Type)
                            {
                                case FHSyncResponseUpdatesData.FHSyncResponseUpdatesDataType.applied:
                                    OnSyncNotification(crashedUpdate.Uid, SyncNotification.REMOTE_UPDATE_APPLIED, crashedUpdate.ToString());
                                    break;
                                case FHSyncResponseUpdatesData.FHSyncResponseUpdatesDataType.failed:
                                    OnSyncNotification(crashedUpdate.Uid, SyncNotification.REMOTE_UPDATE_FAILED, crashedUpdate.ToString());
                                    break;
                                case FHSyncResponseUpdatesData.FHSyncResponseUpdatesDataType.collisions:
                                    OnSyncNotification(crashedUpdate.Uid, SyncNotification.COLLISION_DETECTED, crashedUpdate.ToString());
                                    break;
                                default:
                                    break;
                            }
                            processed = true;
                        }
                    } 
                }
                if(!processed){
                    //no word on our crashed upate - increment a counter to reflect another sync that did not give us any updates on our crashed record
                    pendingRecord.IncrementCrashCount();
                    if(pendingRecord.CrashedCount > this.SyncConfig.CrashedCountWait){
                        DebugLog("Crashed inflight pending record has reached CrashedCount limit");
                        if(this.SyncConfig.ResendCrashedUpdated){
                            DebugLog("Retrying crashed inflight pending record");
                            pendingRecord.ResetCrashStatus();
                        } else {
                            DebugLog("Deleting crashed inflight pending record");
                            this.pendingRecords.Delete(pendingRecordKey);
                        }
                    }
                }
            }
        }

        private void UpdateDelayedFromNewData(FHSyncResponseData syncResData){
            Dictionary<string, FHSyncPendingRecord> localPendingRecords = this.pendingRecords.List();
            foreach (string pendingRecordKey in localPendingRecords.Keys)
            {
                FHSyncPendingRecord pendingRecord = localPendingRecords[pendingRecordKey];
                if(pendingRecord.Delayed && null != pendingRecord.Waiting){
                    DebugLog("Found delayed pending record uid = " + pendingRecord.Uid + " :: hash=" + pendingRecord.GetHashValue());
                    FHSyncResponseUpdatesData waitingRecord = syncResData.GetUpdateByHash(pendingRecord.Waiting);
                    if(null != waitingRecord){
                        DebugLog("Waiting pending record is resolved rec =" + waitingRecord.ToString());
                        pendingRecord.ResetDelayed();
                    }
                }
            }
        }

        private void UpdateMetaFromNewData(FHSyncResponseData syncResData)
        {
            FHSyncMetaData metaData = this.MetaData;
            foreach (string metaDataKey in this.MetaData.Keys)
            {
                string pendingHash = metaData.GetMetaDataAsString(metaDataKey, "pendingUid");
                string previousPendingHash = metaData.GetMetaDataAsString(metaDataKey, "previousPendingUid");
                DebugLog("Found metadata with uid = " + metaDataKey + " :: pendingHash = " + pendingHash + " :: previousPendingHash " + previousPendingHash);
                bool previousPendingResolved = true;
                bool pendingResolved = true;
                if(null != previousPendingHash){
                    //we have previous pending in meta data, see if it's resolved
                    previousPendingResolved = false;
                    FHSyncResponseUpdatesData updateFromRes = syncResData.GetUpdateByHash(previousPendingHash);
                    if(null != updateFromRes){
                        DebugLog("Found previousPendingUid in meta data resolved - resolved = " + updateFromRes.ToString());
                        //the previous pending is resolved in the cloud
                        metaData.DeleteMetaData(metaDataKey, "previousPendingUid");
                        previousPendingResolved = true;
                    }
                }
                if(null != pendingHash){
                    //we have current pending in meta data, see if it's resolved
                    pendingResolved = false;
                    FHSyncResponseUpdatesData updateFromRes = syncResData.GetUpdateByHash(pendingHash);
                    if(null != updateFromRes){
                        DebugLog("Found pendingUid in meta data resolved - resolved = " + updateFromRes.ToString());
                        //the current pending is resolved in the cloud
                        metaData.DeleteMetaData(metaDataKey, "pendingUid");
                        pendingResolved = true;
                    }

                }

                if(pendingResolved && previousPendingResolved){
                    DebugLog("both previous and current pendings are resolved for meta data with uid " + metaDataKey + ". Delete it");
                    metaData.Delete(metaDataKey);
                }
            }
        }

        private void UpdateNewDataFromInFlight(FHSyncResponseData syncResData)
        {
            if(null != syncResData.Records){
                Dictionary<string, FHSyncPendingRecord> localPendingRecords = this.pendingRecords.List();
                foreach (string pendingRecordKey in localPendingRecords.Keys)
                {
                    FHSyncPendingRecord pendingRecord = localPendingRecords[pendingRecordKey];
                    if(pendingRecord.InFlight){
                        FHSyncResponseUpdatesData updatedPending = syncResData.GetUpdateByHash(pendingRecordKey);
                        if(null == updatedPending){
                            DebugLog("Found inFlight pending record -> action =" + pendingRecord.Action + " :: uid = " + pendingRecord.Uid + " :: hash = " + pendingRecord.GetHashValue());
                            FHSyncDataRecord newRecord = syncResData.GetRemoteRecord(pendingRecord.Uid);
                            if(pendingRecord.Action.Equals("update") && null != newRecord){
                                newRecord = pendingRecord.PostData;
                            } else if(pendingRecord.Action.Equals("delete") && null != newRecord){
                                syncResData.Records.Remove(pendingRecord.Uid);
                            } else if(pendingRecord.Action.Equals("create")){
                                DebugLog("re adding pending create to incomming dataset");
                                FHSyncDataRecord createRecordData = pendingRecord.PostData.Clone();
                                syncResData.Records[pendingRecord.Uid] = createRecordData;
                            }
                        }
                    }    
                }
            }
        }

        private void UpdateNewDataFromPending(FHSyncResponseData syncResData)
        {
            if(null != syncResData.Records){
                Dictionary<string, FHSyncPendingRecord> localPendingRecords = this.pendingRecords.List();
                foreach (string pendingRecordKey in localPendingRecords.Keys)
                {
                    FHSyncPendingRecord pendingRecord = localPendingRecords[pendingRecordKey];
                    if(!pendingRecord.InFlight){
                        DebugLog("Found non inFlight record -> action =" + pendingRecord.Action + " :: uid = " + pendingRecord.Uid + " :: hash = " + pendingRecord.GetHashValue());
                        FHSyncDataRecord newRecord = syncResData.GetRemoteRecord(pendingRecord.Uid);
                        if(pendingRecord.Action.Equals("update") && null != newRecord){
                            newRecord = pendingRecord.PostData;
                        } else if(pendingRecord.Action.Equals("delete") && null != newRecord){
                            syncResData.Records.Remove(pendingRecord.Uid);
                        } else if(pendingRecord.Action.Equals("create")){
                            DebugLog("re adding pending create to incomming dataset");
                            FHSyncDataRecord createRecordData = pendingRecord.PostData.Clone();
                            syncResData.Records[pendingRecord.Uid] = createRecordData;
                        }
                    }    
                }
            }
        }

        private void UpdateLocalDatasetFromRemote(FHSyncResponseData syncResData)
        {
            IDataStore<FHSyncDataRecord> anotherDataStore = new InMemoryDataStore<FHSyncDataRecord>();
            foreach(var item in syncResData.Records){
                anotherDataStore.Insert(item.Key, item.Value);
            }
            anotherDataStore.PersistPath = this.dataRecords.PersistPath;
            this.dataRecords = anotherDataStore;
            this.HashValue = syncResData.Hash;
            this.OnSyncNotification(syncResData.Hash, SyncNotification.DELTA_RECEIVED, "full dataset");
        }

        private void ProcessUpdatesFromRemote(FHSyncResponseData syncResData)
        {
            List<FHSyncResponseUpdatesData> acks = new List<FHSyncResponseUpdatesData>();
            foreach(string key in syncResData.Updates.Keys){
                Dictionary<string, FHSyncResponseUpdatesData> updates = syncResData.Updates[key];
                foreach(var item in updates){
                    SyncNotification notification = default(SyncNotification);
                    FHSyncResponseUpdatesData update = item.Value;
                    acks.Add(update);
                    FHSyncPendingRecord pending = this.pendingRecords.Get(item.Key);
                    if(pending.InFlight && !pending.Crashed){
                        this.pendingRecords.Delete(item.Key);
                        switch (update.Type)
                        {
                            case FHSyncResponseUpdatesData.FHSyncResponseUpdatesDataType.applied:
                                notification = SyncNotification.REMOTE_UPDATE_APPLIED;
                                break;
                            case FHSyncResponseUpdatesData.FHSyncResponseUpdatesDataType.failed:
                                notification = SyncNotification.REMOTE_UPDATE_FAILED;
                                break;
                            case FHSyncResponseUpdatesData.FHSyncResponseUpdatesDataType.collisions:
                                notification = SyncNotification.COLLISION_DETECTED;
                                break;
                            default:
                                break;
                        }
                        this.OnSyncNotification(update.Uid, notification, update.ToString());
                    }
                }
            }
            this.Acknowledgements = acks;
        }


        private void MarkInFlightAsCrased(){
            foreach (var item in this.pendingRecords.List())
            {
                FHSyncPendingRecord pendingRecord = item.Value;
                if(pendingRecord.InFlight){
                    DebugLog("Marking in flight pending record as crashed : " + item.Key);
                    pendingRecord.Crashed = true;
                }
            }
        }

        private void DebugLog(string message, [CallerMemberName] string methodName = "")
        {
            string logMessage = string.Format("{0} - {1}", methodName, message);
            logger.d(LOG_TAG, logMessage, null);
        }

        private async Task<FHResponse> DoCloudCall(object syncParams)
        {
            if(this.SyncConfig.SyncCloud == FHSyncConfig.SyncCloudType.AUTO){
                await CheckSyncCloudType();
            }

            if(this.SyncConfig.SyncCloud == FHSyncConfig.SyncCloudType.MBBAS) {
                string service = string.Format("sync/{0}", this.DatasetId);
                FHResponse res = await FH.Mbaas(service, syncParams);
                return res;
            } else {
                FHResponse res = await FH.Act(this.DatasetId, syncParams);
                return res;
            }
        }

        private async Task CheckSyncCloudType()
        {
            Dictionary<string, object> actParams = new Dictionary<string, object>();
            actParams.Add("fh", "sync");
            FHResponse actRes = await FH.Act(this.DatasetId, actParams);
            if(actRes.StatusCode == HttpStatusCode.OK || actRes.StatusCode == HttpStatusCode.InternalServerError){
                this.SyncConfig.SyncCloud = FHSyncConfig.SyncCloudType.LEGACY;
            } else {
                this.SyncConfig.SyncCloud = FHSyncConfig.SyncCloudType.MBBAS;
            }
        }




        /// <summary>
        /// Persist the dataset
        /// </summary>
        protected void Save()
        {
            this.dataRecords.Save();
            this.pendingRecords.Save();
            string syncClientMeta = FHSyncUtils.GetDataFilePath(this.DatasetId, PERSIST_FILE_NAME);
            IIOService iosService = ServiceFinder.Resolve<IIOService>();
            string content = FHSyncUtils.SerializeObject(this);
            iosService.WriteFile(syncClientMeta, content);
        }

        private static FHSyncDataset<X> LoadExistingDataSet<X>(string syncClientMetaFile, string datasetId) where X : IFHSyncModel
        {
            FHSyncDataset<X> result = null;
            IIOService ioService = ServiceFinder.Resolve<IIOService>();
            if (ioService.Exists(syncClientMetaFile))
            {
                string content = ioService.ReadFile(syncClientMetaFile);
                if (!string.IsNullOrEmpty(content))
                {
                    try
                    {
                        FHSyncDataset<X> syncDataset = (FHSyncDataset<X>)FHSyncUtils.DeserializeObject(content, typeof(FHSyncDataset<X>));
                        if (null != syncDataset)
                        {
                            result = LoadDataForDataset<X>(syncDataset);
                        }
                    }
                    catch (Exception ex)
                    {
                        logger.d(LOG_TAG, "Failed to load existing dataset", ex);
                        throw ex;
                    }
                }
            }
            return result;
        }

        private static FHSyncDataset<X> LoadDataForDataset<X>(FHSyncDataset<X> dataSet) where X: IFHSyncModel
        {
            string datasetFile = FHSyncUtils.GetDataFilePath(dataSet.DatasetId, ".data.json");
            string pendingdatasetFile = FHSyncUtils.GetDataFilePath(dataSet.DatasetId, ".pending.json");
            if (null != dataSet.SyncConfig)
            {
                string configDataPath = dataSet.SyncConfig.DataPersistanceDir;
                if (!string.IsNullOrEmpty(configDataPath))
                {
                    datasetFile = Path.Combine(configDataPath, dataSet.DatasetId, ".data.json");
                    pendingdatasetFile = Path.Combine(configDataPath, dataSet.DatasetId, ".pending.json");
                }
            }
            dataSet.dataRecords = InMemoryDataStore<FHSyncDataRecord>.Load<FHSyncDataRecord>(datasetFile);
            dataSet.pendingRecords = InMemoryDataStore<FHSyncPendingRecord>.Load<FHSyncPendingRecord>(pendingdatasetFile);
            return dataSet;
        }

        /// <summary>
        /// Send the event notification
        /// </summary>
        /// <param name="datasetId">Dataset identifier.</param>
        /// <param name="uid">Uid.</param>
        /// <param name="code">Code.</param>
        /// <param name="message">Message.</param>
        protected virtual void OnSyncNotification(string uid, SyncNotification code, string message)
        {
            FHSyncNotificationEventArgs args = new FHSyncNotificationEventArgs
            {
                DatasetId = this.DatasetId,
                Uid = uid,
                Code = code.ToString(),
                Message = message
            };
            logger.d(LOG_TAG, "Receive Notification : " + args.ToString(), null);
            EventHandler<FHSyncNotificationEventArgs> handler = null;
            switch (code)
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

        public bool ShouldSync()
        {
            //TODO: implement me!!
            return true;
        }
            

        public class FHSyncLoopParams
        {
            public FHSyncLoopParams()
            {

            }

            [JsonProperty("fn")]
            public string FnName { get; set; }
            [JsonProperty("dataset_id")]
            public string DatasetId { get; set; }
            [JsonProperty("query_params")]
            public IDictionary<string, string> QueryParams { set; get; }
            [JsonProperty("config")]
            public FHSyncConfig SyncConfg { get; set; }
            [JsonProperty("meta_data")]
            public FHSyncMetaData MetaData { get; set; }
            [JsonProperty("data_set")]
            public string Hash { set; get; }
            [JsonProperty("acknowledgements")]
            public List<FHSyncResponseUpdatesData> Acknowledgements { set; get;}
            [JsonProperty("pending")]
            public List<FHSyncPendingRecord> Pendings { set; get; }

            public FHSyncLoopParams(FHSyncDataset<T> dataset)
            {
                this.FnName = "sync";
                this.DatasetId = dataset.DatasetId;
                this.QueryParams = dataset.QueryParams;
                this.SyncConfg = dataset.SyncConfig;
                this.MetaData = dataset.MetaData;
                this.Hash = dataset.HashValue;
                this.Acknowledgements = dataset.Acknowledgements;
                List<FHSyncPendingRecord> pendingRecords = new List<FHSyncPendingRecord>();
                foreach (KeyValuePair<string, FHSyncPendingRecord> item in dataset.pendingRecords.List()) {
                    FHSyncPendingRecord record = item.Value;
                    if(!record.InFlight && !record.Crashed && !record.Delayed) {
                        record.InFlight = true;
                        record.InFlightDate = DateTime.Now;
                        pendingRecords.Add(record);
                    }
                }
                this.Pendings = pendingRecords;
            }

            public override string ToString()
            {
                return FHSyncUtils.SerializeObject(this);
            }
        }

        public class FHSyncRecordsParams
        {
            public FHSyncRecordsParams()
            {
                
            }

            [JsonProperty("fn")]
            public string FnName { get; set; }
            [JsonProperty("dataset_id")]
            public string DatasetId { get; set; }
            [JsonProperty("query_params")]
            public IDictionary<string, string> QueryParams { set; get; }
            [JsonProperty("clientRecs")]
            Dictionary<string, string> ClientRecords { set; get; }

            public FHSyncRecordsParams(FHSyncDataset<T> dataset)
            {
                this.FnName = "syncRecords";
                this.DatasetId = dataset.DatasetId;
                this.QueryParams = dataset.QueryParams;
                Dictionary<string, string> records = new Dictionary<string, string>();
                foreach(var item in dataset.dataRecords.List()){
                    records.Add(item.Value.Uid, item.Value.HashValue);
                }
                this.ClientRecords = records;
            }

        }
    }



    public class FHSyncNotificationEventArgs : EventArgs
    {
        public string DatasetId { set; get; }

        public string Uid { get; set; }

        public string Code { get; set; }

        public string Message { get; set; }

        public override string ToString()
        {
            return string.Format("[FHSyncNotificationEventArgs: DatasetId={0}, Uid={1}, Code={2}, Message={3}]", DatasetId, Uid, Code, Message);
        }
    }

    public class FHSyncMetaData
    {
        public FHSyncMetaData()
        {

        }

        public Dictionary<string, Dictionary<string, string>> metaData { set; get; }

        private Dictionary<string, string> GetDict(string uid)
        {
            Dictionary<string, string> dict = null;
            metaData.TryGetValue(uid, out dict);
            if (null == dict)
                {
                    dict = new Dictionary<string, string>();
                    metaData[uid] = dict;
                }
            return dict;
        }

        public void InsertStringMetaData(string uid, string key, string value)
        {
            GetDict(uid);
            metaData[uid][key] = value;
        }

        public void InsertBoolMetaData(string uid, string key, bool value)
        {
            GetDict(uid);
            metaData[uid][key] = value.ToString();
        }

        public string GetMetaDataAsString(string uid, string key)
        {
            Dictionary<string, string> dict = GetDict(uid);
            string value = null;
            dict.TryGetValue(key, out value);
            return value;
        }

        public bool GetMetaDataAsBool(string uid, string key)
        {
            string val = GetMetaDataAsString(uid, key);
            if (null != val)
                {
                    return Boolean.Parse(val);
                }
            else
                {
                    return false;
                }
        }

        public Dictionary<string, Dictionary<string, string>>.KeyCollection Keys 
        {
            get {
                return this.metaData.Keys;
            }
        }

        public void DeleteMetaData(string uid, string key)
        {
            Dictionary<string, string>  dict = GetDict(uid);
            if(dict.ContainsKey(key)){
                dict.Remove(key);
            }
        }

        public void Delete(string uid)
        {
            if(metaData.ContainsKey(uid)){
                metaData.Remove(uid);
            }
        }

    }

    public class FHSyncResponseData
    {
        public FHSyncResponseData()
        {

        }

        [JsonProperty("records")]
        public Dictionary<string, FHSyncDataRecord> Records { set; get; }

        [JsonProperty("updates")]
        public Dictionary<string, Dictionary<string, FHSyncResponseUpdatesData>> Updates { set; get; }

        [JsonProperty("hash")]
        public string Hash { set; get; }

        public FHSyncResponseUpdatesData GetAppliedUpdates(string key)
        {
            if(null != this.Updates && this.Updates.Count > 0){
                if(this.Updates.ContainsKey("applied")){
                    Dictionary<string, FHSyncResponseUpdatesData> appliedRecords = this.Updates["applied"];
                    if(appliedRecords.ContainsKey(key)){
                        return appliedRecords[key];
                    }
                }
            }
            return null;
        }

        public FHSyncDataRecord GetRemoteRecord(string key)
        {
            if(null != this.Records && this.Records.Count > 0){
                if(this.Records.ContainsKey(key)){
                    return this.Records[key];
                }
            }
            return null;
        }

        public FHSyncResponseUpdatesData GetUpdateByHash(string hash){
            if(null != this.Updates && this.Updates.ContainsKey("hashes")){
                Dictionary<string, FHSyncResponseUpdatesData> hashes = this.Updates["hashes"];
                return hashes[hash];
            }
            return null;
        }
    }

    public class FHSyncResponseUpdatesData
    {
        public FHSyncResponseUpdatesData()
        {

        }

        public enum FHSyncResponseUpdatesDataType
        {
            applied,
            failed,
            collisions
        }

        [JsonProperty("cuid")]
        public string Cuid { set; get; }
        [JsonProperty("type")]
        [JsonConverter(typeof(StringEnumConverter))]
        public FHSyncResponseUpdatesDataType Type { set; get; }
        [JsonProperty("action")]
        public string Action { set; get; }
        [JsonProperty("hash")]
        public string Hash { set; get; }
        [JsonProperty("uid")]
        public string Uid { set; get; }
        [JsonProperty("message")]
        public string Message { set; get; }
    }

    public class FHSyncRecordsResponseData
    {
        public FHSyncRecordsResponseData()
        {

        }

        [JsonProperty("hash")]
        public string Hash { set; get; }

        [JsonProperty("create")]
        public Dictionary<string, FHSyncDataRecord> CreatedRecords { set; get; }

        [JsonProperty("update")]
        public Dictionary<string, FHSyncDataRecord> UpdatedRecords { set; get; }

        [JsonProperty("delete")]
        public Dictionary<string, FHSyncDataRecord> DeletedRecords { set; get; }

    }
        
}

