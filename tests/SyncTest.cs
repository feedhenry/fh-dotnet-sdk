using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FHSDK;
using FHSDK.Services;
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
            var dataset = NoTransmittingDataset<TaskModel>.Build<TaskModel>("dataset", new FHSyncConfig(), null, null);
            dataset.Create(task);

            //when
            var should = dataset.ShouldSync();

            //then
            Assert.True(should);
            Assert.Empty(dataset.GetPendingRecords().List());
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

    public class NoTransmittingDataset<T> : FHSyncDataset<T> where T : IFHSyncModel
    {
        protected override Task<FHResponse> DoCloudCall(object syncParams)
        {
            SyncParams = syncParams;
            return base.DoCloudCall(syncParams);
        }

        public object SyncParams { get; set; }
    }
}
