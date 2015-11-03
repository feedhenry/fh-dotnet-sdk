using System.Threading.Tasks;
using FHSDK;
using FHSDK.Sync;
using FHSDKTestShared;
using tests.Mocks;
using Xunit;

namespace tests
{
    public class SyncTest
    {
        [Fact]
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
            Assert.NotNull(savedTask.UID);
            Assert.Equal(1, dataset.GetPendingRecords().List().Count);

            var taskRead = dataset.Read(savedTask.UID);
            Assert.NotNull(taskRead);
            Assert.Equal(taskName, taskRead.TaksName);
        }

        [Fact]
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
            Assert.True(shouldSync);
            Assert.Empty(dataset.GetPendingRecords().List());
        }

        [Fact]
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
            Assert.NotNull(readTask);
            Assert.Equal(name, readTask.TaksName);

            //when
            dataset.MockResponse = dataset.AwkRespone;
            await dataset.StartSyncLoop();

            //then
            var list = dataset.List();
            Assert.Equal(1, list.Count);
            Assert.Equal(name, list[0].TaksName);
        }
    }
}