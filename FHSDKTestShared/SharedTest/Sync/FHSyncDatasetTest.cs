using NUnit.Framework;
using System;
using FHSDK;
using System.Net;
using System.Diagnostics;
using FHSDK.Sync;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Threading;

#if __ANDROID__
using FHSDK.Droid;
#elif __IOS__
using FHSDK.Touch;
#endif

namespace FHSDKTestShared
{
    [TestFixture]
    public class FHSyncDatasetTest
    {
        
        private const string DATASET_ID = "tasks";
        private string metaDataFilePath;
        private string dataFilePath;
        private string pendingFilePath;

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
        }

        [Test]
        public async void TestDatasetSync()
        {
            //clear db
            FHResponse setupRes = await FH.Cloud(string.Format("/syncTest/{0}", DATASET_ID), "DELETE", null, null);
            Assert.True(HttpStatusCode.OK.Equals(setupRes.StatusCode));

            FHSyncConfig syncConfig = new FHSyncConfig();
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


            FHSyncDataset<TaskModel> tasksDataset = FHSyncDataset<TaskModel>.Build<TaskModel>(DATASET_ID, syncConfig, null, null);

            //since the dataset is saved on creation, make sure the data files exist
            TestUtils.AssertFileExists(metaDataFilePath);
            TestUtils.AssertFileExists(dataFilePath);
            TestUtils.AssertFileExists(pendingFilePath);

            //Try to create a new Task
            string taskName = "task1";
            TaskModel task = new TaskModel()
            {
                TaksName = taskName,
                Completed = false
            };

            task = tasksDataset.Create(task);

            string taskId = task.UID;
            Debug.WriteLine(string.Format("Created Task Id = {0}", taskId));
            Assert.NotNull(taskId);

            //Now there should be one pending record
            IDataStore<FHSyncPendingRecord<TaskModel>> pendings = tasksDataset.GetPendingRecords();
            int pendingRecordsCount = pendings.List().Count;

            Assert.AreEqual(1, pendingRecordsCount);

            TaskModel taskRead = tasksDataset.Read(taskId);
            Assert.NotNull(taskRead);
            Assert.True(taskRead.TaksName.Equals(taskName));

            Assert.True(tasksDataset.ShouldSync());

            string updatedTaskName = "updatedTask1";
            task.TaksName = updatedTaskName;
            tasksDataset.Update(task);

            //verify data is updated
            TaskModel readUpdated = tasksDataset.Read(taskId);
            Assert.NotNull(readUpdated);
            Assert.True(readUpdated.TaksName.Equals(updatedTaskName));
           
            //test data list
            List<TaskModel> tasks = tasksDataset.List();
            Assert.AreEqual(1, tasks.Count);
            TaskModel listedTask = tasks[0];
            Assert.True(listedTask.TaksName.Equals(updatedTaskName));
            Assert.True(listedTask.Completed == false);
            Assert.True(listedTask.UID.Equals(taskId));


            pendings = tasksDataset.GetPendingRecords();
            Dictionary<string, FHSyncPendingRecord<TaskModel>> pendingsList = pendings.List();
            //updating an existing pending, so there should be still only 1 pending record
            Assert.AreEqual(1, pendingsList.Count);

            FHSyncPendingRecord<TaskModel> pending = pendingsList.First().Value;
            //verify the pendingRecord's postData is the new updated data
            FHSyncDataRecord<TaskModel> postData = pending.PostData;
            Assert.True(updatedTaskName.Equals(postData.Data.TaksName));

            //run the syncLoop
            await tasksDataset.StartSyncLoop();

            //verify the data is created in the remote db
            FHResponse cloudRes = await FH.Cloud(string.Format("/syncTest/{0}", DATASET_ID), "GET", null, null);
            Debug.WriteLine("Got response " + cloudRes.RawResponse);
            Assert.IsNull(cloudRes.Error);
            JObject dbData = cloudRes.GetResponseAsJObject();
            Assert.AreEqual(1, (int)dbData["count"]);
            string taskNameInDb = (string)dbData["list"][0]["fields"]["taskName"];
            Assert.True(taskNameInDb.Equals(updatedTaskName));

            //there should be no pending data
            pendings = tasksDataset.GetPendingRecords();
            Assert.AreEqual(0, pendings.List().Count);

            string newTaskUID = (string)dbData["list"][0]["guid"];
            Assert.False(newTaskUID.Equals(taskId));
            //since the data is now created in db, the uid should be updated
            TaskModel readUpdatedAfterSync = tasksDataset.Read(newTaskUID);
            Assert.NotNull(readUpdatedAfterSync);
            Debug.WriteLine("readUpdatedAfterSync = " + readUpdatedAfterSync.ToString());

            //create a new record in remote db
            TaskModel taskToCreate = new TaskModel()
            {
                TaksName = "anotherTask",
                Completed = false
            };
            FHResponse createRecordReq = await FH.Cloud(string.Format("/syncTest/{0}", DATASET_ID), "POST", null, taskToCreate);
            Debug.WriteLine(string.Format("Got response for creating new record: {0}", createRecordReq.RawResponse));
            Assert.IsNull(createRecordReq.Error);

            Thread.Sleep(1500);
            Assert.True(tasksDataset.ShouldSync());

            //run a sync loop
            await tasksDataset.StartSyncLoop();

            //now we should see the new record is created locally
            List<TaskModel> updatedTaskList = tasksDataset.List();
            Assert.AreEqual(2, updatedTaskList.Count);
            Debug.WriteLine("updatedTaskList[0] = " + updatedTaskList[0].ToString());
            Debug.WriteLine("updatedTaskList[1] = " + updatedTaskList[1].ToString());

            FHSyncDataset<TaskModel> anotherDataset = FHSyncDataset<TaskModel>.Build<TaskModel>(DATASET_ID, syncConfig, null, null);
            List<TaskModel> tasksListInAnotherDataset = anotherDataset.List();
            Assert.AreEqual(2, tasksListInAnotherDataset.Count);
            foreach (TaskModel taskInAnotherDataset in tasksListInAnotherDataset)
            {
                Assert.NotNull(taskInAnotherDataset.UID);
                Assert.True(taskInAnotherDataset.TaksName.Equals("updatedTask1") || taskInAnotherDataset.TaksName.Equals("anotherTask"));
            }
        }




    }
}

