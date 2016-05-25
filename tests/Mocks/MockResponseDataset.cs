using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using FHSDK;
using FHSDK.Services;
using FHSDK.Services.Hash;
using FHSDK.Sync;

namespace tests.Mocks
{
    public class MockResponseDataset<T> : FHSyncDataset<T> where T : IFHSyncModel
    {
        public readonly FHResponse AppliedCreateResponse = new FHResponse(HttpStatusCode.OK,
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

        public readonly FHResponse AwkRespone = new FHResponse(HttpStatusCode.OK, 
            @"{
            	""hash"": ""97d170e1550eee4afc0af065b78cda302a97674c"",
	            ""updates"": {}
            }");

        public readonly FHResponse NoUpdates = new FHResponse(HttpStatusCode.OK, 
            @"{
	            ""updates"": {}
            }");

        public readonly FHResponse RemoteCreatedResponse = new FHResponse(HttpStatusCode.OK,
            @"{
               ""create"":{
                  ""561b7cf1810880dc18000029"":{
                     ""data"":{
                        ""taskName"":""123"",
                        ""completed"": true
                     },
                     ""hash"":""6b749553cc45d1344cc12426f70d6ff584284bf8""
                  }
               },
               ""update"":{

               },
               ""delete"":{
                  ""6b749553cc45d1344cc12426f70d6ff584284bf8"":{

                  }
               },
               ""hash"":""d8a15c0831eb10cb99766f4d976250e3a3a49de3""
            }");

        public MockResponseDataset(string datasetId)
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

            ServiceFinder.RegisterType<IHashService, TestHasher>();
            Save();
        }

        public FHResponse MockResponse { private get; set; }
        public object SyncParams { get; private set; }
        public Type KeepSyncParamType { private get; set; }

        protected override Task<FHResponse> DoCloudCall(object syncParams)
        {
            if (syncParams.GetType() == KeepSyncParamType)
            {
                SyncParams = syncParams;
            }
            return Task.Factory.StartNew(() => MockResponse);
        }


        private class TestHasher : IHashService
        {
            public string GenerateSha1Hash(string str)
            {
                return "072f620bdd0c5285d60b1be3dc0600f07585d21c";
            }
        }
    }
}