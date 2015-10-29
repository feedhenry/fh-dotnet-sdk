using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using FHSDK;
using FHSDK.Services;
using FHSDK.Services.Hash;
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
            ServiceFinder.RegisterType<IHashService, TestHasher>();
            TaskModel task;
            InitDataset("test", out task);
            var dataset = new NoTransmittingDataset<TaskModel>("dataset");
            task = dataset.Create(task);

            //when
            var should = dataset.ShouldSync();
            dataset.MockResponse = new FHResponse(HttpStatusCode.OK,
            @"{
	            ""hash"": ""084540e99a0179151841b2548daa6d98d5a81d89"",
	            ""updates"": {
		            ""hashes"": {
			            ""072f620bdd0c5285d60b1be3dc0600f07585d21c"": {
				            ""cuid"": ""922f3ef3e8c0ad617e69eecb70910917"",
				            ""type"": ""applied"",
				            ""action"": ""create"",
				            ""hash"": ""072f620bdd0c5285d60b1be3dc0600f07585d21c"",
				            ""uid"": ""5630e184e48dca6421000002"",
				            ""msg"": ""''""
			            }
		            },
		            ""applied"": {
			            ""072f620bdd0c5285d60b1be3dc0600f07585d21c"": {
				            ""cuid"": ""922f3ef3e8c0ad617e69eecb70910917"",
				            ""type"": ""applied"",
				            ""action"": ""create"",
				            ""hash"": ""072f620bdd0c5285d60b1be3dc0600f07585d21c"",
				            ""uid"": ""072f620bdd0c5285d60b1be3dc0600f07585d21c"",
				            ""msg"": ""''""
			            }
		            }
	            }
            }");
            await dataset.StartSyncLoop();

            //then
            Assert.True(should);
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

    public class TestHasher : IHashService
    {
        public string GenerateSha1Hash(string str)
        {
            return "072f620bdd0c5285d60b1be3dc0600f07585d21c";
        }
    }

    public class NoTransmittingDataset<T> : FHSyncDataset<T> where T : IFHSyncModel
    {
        public NoTransmittingDataset(string datasetId)
        {
            DatasetId = datasetId;
            SyncConfig = new FHSyncConfig {SyncCloud = FHSyncConfig.SyncCloudType.Mbbas};
            MetaData = new FHSyncMetaData();
            QueryParams = new Dictionary<string, string>();
            UidMapping = new Dictionary<string, string>();
            dataRecords = new InMemoryDataStore<FHSyncDataRecord<T>>
            {
                PersistPath = GetPersistFilePathForDataset(SyncConfig, datasetId, DATA_PERSIST_FILE_NAME)
            };
            pendingRecords = new InMemoryDataStore<FHSyncPendingRecord<T>>
            {
                PersistPath = GetPersistFilePathForDataset(SyncConfig, datasetId, PENDING_DATA_PERSIST_FILE_NAME)
            };

            Save();
        }

        protected override Task<FHResponse> DoCloudCall(object syncParams)
        {
            SyncParams = syncParams;
            return Task.Factory.StartNew(() => MockResponse);
        }

        public object SyncParams { get; set; }
        public FHResponse MockResponse { get; set; }
    }
}
