using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
            var dataset = FHSyncDataset<TaskModel>.Build<TaskModel>("dataset", new FHSyncConfig(), null, null);

            const string taskName = "task1";
            var task = new TaskModel
            {
                TaksName = taskName,
                Completed = false
            };

            //when
            var savedTask = dataset.Create(task);

            //then
            Assert.NotNull(savedTask.UID);
            Assert.Equal(1, dataset.GetPendingRecords().List().Count);

            var taskRead = dataset.Read(savedTask.UID);
            Assert.NotNull(taskRead);
            Assert.Equal(taskName, taskRead.TaksName);

        }
    }
}
