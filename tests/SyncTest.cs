using System.Threading.Tasks;
using FHSDK;
using FHSDK.Sync;
using FHSDKTestShared;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using tests.Mocks;

namespace tests
{
    [TestClass]
    public class SyncTest
    {
        [TestMethod]
        public async Task ShouldCreatePendingTaskForCreate()
        {
            //given
            await FHClient.Init();
            const string taskName = "task1";
            var dataset1 = FHSyncDataset<TaskModel>.Build<TaskModel>("dataset", new FHSyncConfig(), null, null);
            var task = new TaskModel
            {
                TaksName = taskName
            };
            var dataset = dataset1;

            //when
            var savedTask = dataset.Create(task);

            //then
            Assert.IsNotNull(savedTask.UID);
            Assert.AreEqual(1, dataset.GetPendingRecords().List().Count);

            var taskRead = dataset.Read(savedTask.UID);
            Assert.IsNotNull(taskRead);
            Assert.AreEqual(taskName, taskRead.TaksName);
        }

        [TestMethod]
        public async Task ShouldUploadPendingEdits()
        {
            //given
            await FHClient.Init();

            var dataset = new MockResponseDataset<TaskModel>("dataset");
            dataset.MockResponse = dataset.AppliedCreateResponse;
            var task = new TaskModel
            {
                TaksName = "test"
            };

            dataset.Create(task);

            //when
            var shouldSync = dataset.ShouldSync();
            await dataset.StartSyncLoop();

            //then
            Assert.IsTrue(shouldSync);
            Assert.AreEqual(0, dataset.GetPendingRecords().List().Count);
        }

        [TestMethod]
        public async Task ShouldCreateUpdate()
        {
            //given
            await FHClient.Init();
            var dataset = new MockResponseDataset<TaskModel>("dataset");
            var task = new TaskModel
            {
                TaksName = "test"
            };
            
            //when
            task = dataset.Create(task);
            const string name = "super";
            task.TaksName = name;
            dataset.Update(task);

            //then
            var readTask = dataset.Read(task.UID);
            Assert.IsNotNull(readTask);
            Assert.AreEqual(name, readTask.TaksName);

            //when
            dataset.MockResponse = dataset.AwkRespone;
            await dataset.StartSyncLoop();

            //then
            var list = dataset.List();
            Assert.AreEqual(1, list.Count);
            Assert.AreEqual(name, list[0].TaksName);
        }
    }
}