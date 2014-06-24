using NUnit.Framework;
using System;
using FHSDK;
using System.IO;
using System.Net;
using FHSDK.Sync;
using System.Collections.Generic;
using System.Threading;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;

#if __ANDROID__
using FHSDK.Droid;
#elif __IOS__
using FHSDK.Touch;
#endif

namespace FHSDKTestShared
{
    [TestFixture()]
    public class FHSyncClientTest
    {
        private const string DATASET_ID = "tasks";
        private string metaDataFilePath;
        private string dataFilePath;
        private string pendingFilePath;
        FHSyncClient syncClient;

        [SetUp]
        public void SetUp()
        {
            FHClient.Init();
            FH.SetLogLevel(1);

        }

        [TearDown]
        public void TearDown()
        {
            TestUtils.DeleteFileIfExists(metaDataFilePath);
            TestUtils.DeleteFileIfExists(dataFilePath);
            TestUtils.DeleteFileIfExists(pendingFilePath);
            if(null != syncClient){
                syncClient.StopAll();
            }
        }


        [Test]
        public async void TestCase()
        {
            //clear db
            FHResponse setupRes = await FH.Cloud(string.Format("/syncTest/{0}", DATASET_ID), "DELETE", null, null);
            Assert.True(HttpStatusCode.OK.Equals(setupRes.StatusCode));

            FHSyncConfig syncConfig = new FHSyncConfig();
            syncConfig.SyncActive = false;
            syncConfig.SyncFrequency = 1;
            syncConfig.AutoSyncLocalUpdates = true;
            syncConfig.SyncCloud = FHSyncConfig.SyncCloudType.MBBAS;

            //make sure no existing data file exist
            metaDataFilePath = FHSyncUtils.GetDataFilePath(DATASET_ID, ".sync.json");
            dataFilePath = FHSyncUtils.GetDataFilePath(DATASET_ID, ".data.json");
            pendingFilePath = FHSyncUtils.GetDataFilePath(DATASET_ID, ".pendings.json");

            TestUtils.DeleteFileIfExists(metaDataFilePath);
            TestUtils.DeleteFileIfExists(dataFilePath);
            TestUtils.DeleteFileIfExists(pendingFilePath);

            Assert.False(File.Exists(metaDataFilePath));
            Assert.False(File.Exists(dataFilePath));
            Assert.False(File.Exists(pendingFilePath));

            syncClient = FHSyncClient.GetInstance();
            syncClient.Initialise(syncConfig);
            syncClient.Manage<TaskModel>(DATASET_ID, syncConfig, null);

            bool syncStarted = false;
            bool syncCompleted = false;

            syncClient.SyncStarted += (object sender, FHSyncNotificationEventArgs e) => {
                if(e.DatasetId.Equals(DATASET_ID)){
                    syncStarted = true;
                }
            };

            syncClient.SyncCompleted += (object sender, FHSyncNotificationEventArgs e) => {
                if(e.DatasetId.Equals(DATASET_ID)){
                    syncCompleted = true;
                }
            };

            List<TaskModel> tasks = syncClient.List<TaskModel>(DATASET_ID);
            Assert.AreEqual(0, tasks.Count);

            TaskModel newTask = new TaskModel()
            {
                TaksName = "task1",
                Completed = false
            };
            newTask = syncClient.Create<TaskModel>(DATASET_ID, newTask);
            //syncConfig.SyncActive = true;
            Assert.NotNull(newTask.UID);

            syncClient.ForceSync<TaskModel>(DATASET_ID);

            Thread.Sleep(1500);

            FHResponse cloudRes = await FH.Cloud(string.Format("/syncTest/{0}", DATASET_ID), "GET", null, null);
            Assert.IsNull(cloudRes.Error);
            JObject dbData = cloudRes.GetResponseAsJObject();
            Assert.AreEqual(1, (int)dbData["count"]);
            string taskNameInDb = (string)dbData["list"][0]["fields"]["taskName"];
            Assert.True(taskNameInDb.Equals("task1"));

            Assert.IsTrue(syncStarted);
            Assert.IsTrue(syncCompleted);
        }
    }
}

