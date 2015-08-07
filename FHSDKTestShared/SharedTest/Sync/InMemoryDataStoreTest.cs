#if __ANDROID__
using FHSDK.Droid;
#elif __IOS__
using FHSDK.Touch;
#endif
using System.Diagnostics;
using System.IO;
using FHSDK.Services;
using FHSDK.Services.Data;
using FHSDK.Sync;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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
    public class InMemoryDataStoreTest
    {
        private string _dataPersistDir;
        private string _dataPersistFile;
        private IIOService _ioService;

        [SetUp]
        public void SetUp()
        {
            FHClient.Init();
            _ioService = ServiceFinder.Resolve<IIOService>();
            var dataDir = _ioService.GetDataPersistDir();
            _dataPersistDir = Path.Combine(dataDir, "syncTestDir");
            if (Directory.Exists(_dataPersistDir))
            {
                Directory.Delete(_dataPersistDir);
            }
            _dataPersistFile = Path.Combine(_dataPersistDir, ".test_data_file");
            Debug.WriteLine("Data persist path = {0}", _dataPersistFile);
        }

        [TearDown]
        public void TearDown()
        {
            if (File.Exists(_dataPersistFile))
            {
                File.Delete(_dataPersistFile);
            }
            if (Directory.Exists(_dataPersistDir))
            {
                Directory.Delete(_dataPersistDir);
            }
        }

        [Test]
        public void TestInMemoryDataStore()
        {
            Assert.IsFalse(File.Exists(_dataPersistFile));
            IDataStore<JObject> dataStore = new InMemoryDataStore<JObject>();
            dataStore.PersistPath = _dataPersistFile;
            var key1 = "testkey1";
            var key2 = "testkey2";
            var json1 = TestUtils.GenerateJson();
            var json2 = TestUtils.GenerateJson();
            var json3 = TestUtils.GenerateJson();

            dataStore.Insert(key1, json1);
            dataStore.Insert(key2, json2);

            var listResult = dataStore.List();
            Assert.AreEqual(2, listResult.Count);
            Assert.IsNotNull(listResult[key1]);
            Assert.IsNotNull(listResult[key2]);

            var getResult = dataStore.Get(key1);
            Assert.IsTrue(JsonConvert.SerializeObject(getResult).Equals(JsonConvert.SerializeObject(json1)));


            dataStore.Insert(key2, json3);
            var getResult2 = dataStore.Get(key2);
            Assert.IsTrue(JsonConvert.SerializeObject(getResult2).Equals(JsonConvert.SerializeObject(json3)));

            dataStore.Save();
            Assert.IsTrue(File.Exists(_dataPersistFile));

            string savedFileContent;
            var reader = new StreamReader(_dataPersistFile);
            savedFileContent = reader.ReadToEnd();
            reader.Close();
            Debug.WriteLine("Save file content = {0}", savedFileContent);
            Assert.IsTrue(savedFileContent.Length > 0);

            IDataStore<JObject> loadedDataStore = InMemoryDataStore<JObject>.Load<JObject>(_dataPersistFile);
            var listResult2 = loadedDataStore.List();
            Assert.AreEqual(2, listResult2.Count);
            Assert.IsNotNull(listResult2[key1]);
            Assert.IsNotNull(listResult2[key2]);

            var getResult3 = loadedDataStore.Get(key2);
            Assert.IsTrue(JsonConvert.SerializeObject(getResult3).Equals(JsonConvert.SerializeObject(json3)));

            loadedDataStore.Delete(key1);
            listResult2 = loadedDataStore.List();
            Assert.AreEqual(1, listResult2.Count);
            Assert.IsNull(loadedDataStore.Get(key1));
        }
    }
}