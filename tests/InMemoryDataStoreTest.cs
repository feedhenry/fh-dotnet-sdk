using System.IO;
using FHSDK;
using FHSDK.Services;
using FHSDK.Services.Data;
using FHSDK.Sync;
using Newtonsoft.Json.Linq;
using Xunit;

namespace tests
{
    public class InMemoryDataStoreTest
    {
        private readonly string _dataPersistFile;
        private readonly IIOService _ioService;

        public InMemoryDataStoreTest()
        {
            FHClient.Init();
            _ioService = ServiceFinder.Resolve<IIOService>();
            var dataDir = _ioService.GetDataPersistDir();
            var dataPersistDir = Path.Combine(dataDir, "syncTestDir");
            _dataPersistFile = Path.Combine(dataPersistDir, ".test_data_file");
        }

        [Fact]
        public void TestInsertIntoDataStore()
        {
            //given
            IDataStore<JObject> dataStore = new InMemoryDataStore<JObject>();
            var obj = new JObject();
            obj["key"] = "value";

            //when
            dataStore.Insert("key1", obj);
            dataStore.Insert("key2", obj);

            //then
            var list = dataStore.List();
            Assert.Equal(2, list.Count);
            Assert.NotNull(list["key1"]);

            var result = dataStore.Get("key1");
            Assert.Equal(obj, result);
        }

        [Fact]
        public void TestSaveDataStoreToJson()
        {
            //given
            IDataStore<JObject> dataStore = new InMemoryDataStore<JObject>();
            dataStore.PersistPath = _dataPersistFile;
            var obj = new JObject();
            obj["key"] = "value";

            //when
            dataStore.Insert("main-key", obj);
            dataStore.Save();

            //then
            Assert.True(_ioService.Exists(_dataPersistFile));
            var content = _ioService.ReadFile(_dataPersistFile);
            Assert.Equal("{\"main-key\":{\"key\":\"value\"}}", content);
        }
    }
}
