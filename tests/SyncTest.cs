using System.Threading.Tasks;
using FHSDK.Sync;
using FHSDKPortable;
using FHSDKTestShared;
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
            TaskModel task;
            var dataset = InitDataset(taskName, out task);

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
            TaskModel task;
            InitDataset("test", out task);
            var dataset = new MockResponseDataset<TaskModel>("dataset");
            dataset.MockResponse = dataset.AppliedCreateResponse;
            dataset.Create(task);

            //when
            var shouldSync = dataset.ShouldSync();
            await dataset.StartSyncLoop();

            //then
            Assert.True(shouldSync);
            Assert.Empty(dataset.GetPendingRecords().List());
            //Assert.Equal( , dataset.SyncParams);
        }

        private static FHSyncDataset<TaskModel> InitDataset(string taskName, out TaskModel task)
        {
            var dataset = FHSyncDataset<TaskModel>.Build<TaskModel>("dataset", new FHSyncConfig(), null, null);

            task = new TaskModel
            {
                TaksName = taskName
            };
            return dataset;
        }
    }
}