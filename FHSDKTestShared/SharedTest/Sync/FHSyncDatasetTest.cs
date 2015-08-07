#if __ANDROID__
using FHSDK.Droid;
#elif __IOS__
using FHSDK.Touch;
#endif
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using FHSDK;
using FHSDK.Sync;
#if WINDOWS_PHONE
using TestFixture = Microsoft.VisualStudio.TestPlatform.UnitTestFramework.TestClassAttribute;
using Test = Microsoft.VisualStudio.TestPlatform.UnitTestFramework.TestMethodAttribute;
using SetUp = Microsoft.VisualStudio.TestPlatform.UnitTestFramework.TestInitializeAttribute;
using TearDown = Microsoft.VisualStudio.TestPlatform.UnitTestFramework.TestCleanupAttribute;
using FHSDK.Phone;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;

#else
using NUnit.Framework;
#endif

namespace FHSDKTestShared
{
    [TestFixture]
    public class FHSyncDatasetTest
    {
        private const string DatasetId = "data_tasks";
        private string _dataFilePath;
        private string _metaDataFilePath;
        private string _pendingFilePath;

        [SetUp]
        public void SetUp()
        {
            FHClient.Init();
            FH.SetLogLevel(1);
        }

        [TearDown]
        public void TearDown()
        {
            TestUtils.DeleteFileIfExists(_metaDataFilePath);
            TestUtils.DeleteFileIfExists(_dataFilePath);
            TestUtils.DeleteFileIfExists(_pendingFilePath);
        }

        [Test]
#if WINDOWS_PHONE
        public async Task TestDatasetSync()
#else
        public async void TestDatasetSync()
        #endif
        {
            //clear db
            var setupRes = await FH.Cloud(string.Format("/syncTest/{0}", DatasetId), "DELETE", null, null);
            Assert.IsTrue(HttpStatusCode.OK.Equals(setupRes.StatusCode));

            var syncConfig = new FHSyncConfig();
            syncConfig.SyncFrequency = 1;
            syncConfig.AutoSyncLocalUpdates = true;
            syncConfig.SyncCloud = FHSyncConfig.SyncCloudType.Mbbas;
            syncConfig.CrashedCountWait = 0;
            syncConfig.ResendCrashedUpdated = true;

            //make sure no existing data file exist
            _metaDataFilePath = FHSyncUtils.GetDataFilePath(DatasetId, ".sync.json");
            _dataFilePath = FHSyncUtils.GetDataFilePath(DatasetId, ".data.json");
            _pendingFilePath = FHSyncUtils.GetDataFilePath(DatasetId, ".pendings.json");

            TestUtils.DeleteFileIfExists(_metaDataFilePath);
            TestUtils.DeleteFileIfExists(_dataFilePath);
            TestUtils.DeleteFileIfExists(_pendingFilePath);

            Assert.IsFalse(File.Exists(_metaDataFilePath));
            Assert.IsFalse(File.Exists(_dataFilePath));
            Assert.IsFalse(File.Exists(_pendingFilePath));


            var tasksDataset = FHSyncDataset<TaskModel>.Build<TaskModel>(DatasetId, syncConfig, null, null);

            //since the dataset is saved on creation, make sure the data files exist
            TestUtils.AssertFileExists(_metaDataFilePath);
            TestUtils.AssertFileExists(_dataFilePath);
            TestUtils.AssertFileExists(_pendingFilePath);

            //Try to create a new Task
            var taskName = "task1";
            var task = new TaskModel
            {
                TaksName = taskName,
                Completed = false
            };

            task = tasksDataset.Create(task);

            var taskId = task.UID;
            Debug.WriteLine("Created Task Id = {0}", taskId);
            Assert.IsNotNull(taskId);

            //Now there should be one pending record
            var pendings = tasksDataset.GetPendingRecords();
            var pendingRecordsCount = pendings.List().Count;

            Assert.AreEqual(1, pendingRecordsCount);

            var taskRead = tasksDataset.Read(taskId);
            Assert.IsNotNull(taskRead);
            Assert.IsTrue(taskRead.TaksName.Equals(taskName));

            Assert.IsTrue(tasksDataset.ShouldSync());

            var updatedTaskName = "updatedTask1";
            task.TaksName = updatedTaskName;
            tasksDataset.Update(task);

            //verify data is updated
            var readUpdated = tasksDataset.Read(taskId);
            Assert.IsNotNull(readUpdated);
            Assert.IsTrue(readUpdated.TaksName.Equals(updatedTaskName));

            //test data list
            var tasks = tasksDataset.List();
            Assert.AreEqual(1, tasks.Count);
            var listedTask = tasks[0];
            Assert.IsTrue(listedTask.TaksName.Equals(updatedTaskName));
            Assert.IsTrue(listedTask.Completed == false);
            Assert.IsTrue(listedTask.UID.Equals(taskId));


            pendings = tasksDataset.GetPendingRecords();
            var pendingsList = pendings.List();
            //updating an existing pending, so there should be still only 1 pending record
            Assert.AreEqual(1, pendingsList.Count);

            var pending = pendingsList.First().Value;
            //verify the pendingRecord's postData is the new updated data
            var postData = pending.PostData;
            Assert.IsTrue(updatedTaskName.Equals(postData.Data.TaksName));

            //run the syncLoop
            await tasksDataset.StartSyncLoop();

            //verify the data is created in the remote db
            var cloudRes = await FH.Cloud(string.Format("/syncTest/{0}", DatasetId), "GET", null, null);
            Debug.WriteLine("Got response " + cloudRes.RawResponse);
            Assert.IsNull(cloudRes.Error);
            var dbData = cloudRes.GetResponseAsJObject();
            Assert.AreEqual(1, (int) dbData["count"]);
            var taskNameInDb = (string) dbData["list"][0]["fields"]["taskName"];
            Assert.IsTrue(taskNameInDb.Equals(updatedTaskName));

            //there should be no pending data
            pendings = tasksDataset.GetPendingRecords();
            Assert.AreEqual(0, pendings.List().Count);

            var newTaskUid = (string) dbData["list"][0]["guid"];
            Assert.IsFalse(newTaskUid.Equals(taskId));
            var tasklistAfterUpdate = tasksDataset.List();
            Debug.WriteLine(tasklistAfterUpdate[0].ToString());

            //since the data is now created in db, the uid should be updated
            var readUpdatedAfterSync = tasksDataset.Read(newTaskUid);
            Assert.IsNotNull(readUpdatedAfterSync);
            Debug.WriteLine("readUpdatedAfterSync = " + readUpdatedAfterSync);

            //create a new record in remote db
            var taskToCreate = new TaskModel
            {
                TaksName = "anotherTask",
                Completed = false
            };
            var createRecordReq = await FH.Cloud(string.Format("/syncTest/{0}", DatasetId), "POST", null, taskToCreate);
            Debug.WriteLine("Got response for creating new record: {0}", createRecordReq.RawResponse);
            Assert.IsNull(createRecordReq.Error);

            Thread.Sleep(1500);
            Assert.IsTrue(tasksDataset.ShouldSync());

            //run a sync loop
            await tasksDataset.StartSyncLoop();

            Thread.Sleep(9000);

            await tasksDataset.StartSyncLoop();

            //now we should see the new record is created locally
            var updatedTaskList = tasksDataset.List();
            Assert.AreEqual(2, updatedTaskList.Count);
            Debug.WriteLine("updatedTaskList[0] = " + updatedTaskList[0]);
            Debug.WriteLine("updatedTaskList[1] = " + updatedTaskList[1]);

            //verify that the saved files can be loaded again and will construct the same dataset
            var anotherDataset = FHSyncDataset<TaskModel>.Build<TaskModel>(DatasetId, syncConfig, null, null);
            Assert.IsNotNull(anotherDataset.HashValue);
            var tasksListInAnotherDataset = anotherDataset.List();
            Assert.AreEqual(2, tasksListInAnotherDataset.Count);
            foreach (var taskInAnotherDataset in tasksListInAnotherDataset)
            {
                Assert.IsNotNull(taskInAnotherDataset.UID);
                Assert.IsTrue(taskInAnotherDataset.TaksName.Equals("updatedTask1") ||
                              taskInAnotherDataset.TaksName.Equals("anotherTask"));
            }

            //test some failure cases
            var taskToUpdate = updatedTaskList[0];
            taskToUpdate.Completed = true;

            tasksDataset.Update(taskToUpdate);

            //make sure the next syncLoop will fail
            var setFailureRes = await FH.Cloud("/setFailure/true", "GET", null, null);
            Assert.IsNull(setFailureRes.Error);
            Assert.IsTrue((bool) setFailureRes.GetResponseAsJObject()["current"]);

            //run the syncloop again, this time it will fail
            await tasksDataset.StartSyncLoop();

            pendings = tasksDataset.GetPendingRecords();
            Assert.AreEqual(1, pendings.List().Count);

            var pendingRecord = pendings.List().Values.First();
            Assert.IsTrue(pendingRecord.Crashed);

            setFailureRes = await FH.Cloud("/setFailure/false", "GET", null, null);
            Assert.IsNull(setFailureRes.Error);
            Assert.IsFalse((bool) setFailureRes.GetResponseAsJObject()["current"]);

            //run another sync loop, this time there will be no pendings sent as the current update is crashed
            await tasksDataset.StartSyncLoop();

            //at the end of last loop, since we set the crashCountWait to be 0, the updates should be marked as not-crashed and will be sent in next loop
            await tasksDataset.StartSyncLoop();

            //there should be no more pendings
            pendings = tasksDataset.GetPendingRecords();
            Assert.AreEqual(0, pendings.List().Count);

            //verify the update is sent to the cloud
            var verifyUpdateRes =
                await FH.Cloud(string.Format("/syncTest/{0}/{1}", DatasetId, taskToUpdate.UID), "GET", null, null);
            var taskInDBJson = verifyUpdateRes.GetResponseAsJObject();
            Assert.IsTrue((bool) taskInDBJson["fields"]["completed"]);
        }
    }
}