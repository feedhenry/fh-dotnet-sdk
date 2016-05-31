#if __MOBILE__
using Xunit;
using TestMethod = Xunit.FactAttribute;
#else
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
#endif

using System.IO;
using FHSDK;
using FHSDK.Services;
using FHSDK.Services.Data;
using FHSDK.Sync;
using Newtonsoft.Json.Linq;

namespace tests
{
    [TestClass]
    public class InMemoryDataStoreTest
    {
        private string _dataPersistFile;
        private IIOService _ioService;

		public InMemoryDataStoreTest()
        {
            FHClient.Init();
            _ioService = ServiceFinder.Resolve<IIOService>();
            var dataDir = _ioService.GetDataPersistDir();
            var dataPersistDir = Path.Combine(dataDir, "syncTestDir");
            _dataPersistFile = Path.Combine(dataPersistDir, ".test_data_file");
        }

        [TestMethod]
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
            Assert.AreEqual(2, list.Count);
            Assert.IsNotNull(list["key1"]);

            var result = dataStore.Get("key1");
            Assert.AreEqual(obj, result);
        }

        [TestMethod]
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
            Assert.IsTrue(_ioService.Exists(_dataPersistFile));
            var content = _ioService.ReadFile(_dataPersistFile);
            Assert.AreEqual("{\"main-key\":{\"key\":\"value\"}}", content);
        }
    }
}
