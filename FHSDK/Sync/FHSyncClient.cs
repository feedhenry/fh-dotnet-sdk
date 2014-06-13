using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FHSDK.Sync
{
    public class FHSyncClient
    {
        private static FHSyncClient syncClientInstance;
        private const string LOG_TAG = "FHSyncClient";

        private bool initialised = false;
        private FHSyncConfig globalSyncConfig = new FHSyncConfig();

        private bool keepRunning = true;

        private Dictionary<string, FHSyncDataset<IFHSyncModel>> datasets = new Dictionary<string, FHSyncDataset<IFHSyncModel>>();

        private FHSyncClient()
        {
        }

        private void Initialise(FHSyncConfig syncConfig)
        {
          if(null != syncConfig){
                this.globalSyncConfig = syncConfig;
          }

          if(!initialised){
            this.MonitorTask();
          }
          this.initialised = true;
        }

        private FHSyncDataset<IFHSyncModel> Manage<T>(string datasetId, FHSyncConfig syncConfig, IDictionary<string, string> qp) where T:IFHSyncModel
        {
            FHSyncDataset<IFHSyncModel> dataset = null;
            if(!this.datasets.ContainsKey(datasetId)){
                dataset = (FHSyncDataset<IFHSyncModel>)FHSyncDataset<IFHSyncModel>.Build<IFHSyncModel>(datasetId, syncConfig, qp, null);
                datasets[datasetId] = dataset;
            }else {
                dataset = this.datasets[datasetId];
            }

            return dataset;
        }


        private void CheckDatasets()
        {
            foreach(var dataset in datasets.Values){
                if(!dataset.syncRunning && !dataset.StopSync){
                    if(dataset.ShouldSync()){
                        dataset.syncPending = true;
                    }
                    if(dataset.syncPending){
                        dataset.StartSyncLoop();
                    }
                }
            }
        }

        private void MonitorTask()
        {
            CheckDatasets();
            Task.Delay(TimeSpan.FromSeconds(this.globalSyncConfig.SyncFrequency));
            Task.Run(() =>
            {
                if(keepRunning){
                    MonitorTask();
                }

            });
        }
    }
}

