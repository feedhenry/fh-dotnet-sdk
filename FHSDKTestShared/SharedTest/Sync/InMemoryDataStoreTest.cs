using NUnit.Framework;
using System;

using FHSDK.Services;
using FHSDK.Sync;
using FHSDK;
using System.IO;
using System.Diagnostics;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using Newtonsoft.Json;
#if __ANDROID__
using FHSDK.Droid;
#elif __IOS__
using FHSDK.Touch;
#endif

namespace FHSDKTestShared
{
    [TestFixture]
    public class InMemoryDataStoreTest
    {
        private IIOService ioService;
        private string dataPersistDir = null;
        private string dataPersistFile = null;

        [SetUp]
        public void SetUp()
        {
            FHClient.Init();
            ioService = ServiceFinder.Resolve<IIOService>();
            string dataDir = ioService.GetDataPersistDir();
            dataPersistDir = Path.Combine(dataDir, "syncTestDir");
            if(Directory.Exists(dataPersistDir)){
                Directory.Delete(dataPersistDir);
            }
            dataPersistFile = Path.Combine(dataPersistDir, ".test_data_file");
            Debug.WriteLine(String.Format("Data persist path = {0}", dataPersistFile));
        }

        [TearDown]
        public void TearDown()
        {
            if(File.Exists(dataPersistFile)){
                File.Delete(dataPersistFile);
            }
            if(Directory.Exists(dataPersistDir)){
                Directory.Delete(dataPersistDir);
            }
        }
        
        [Test]
        public void TestInMemoryDataStore()
        {
            Assert.False(File.Exists(dataPersistFile));
            IDataStore<JObject> dataStore = new InMemoryDataStore<JObject>();
            dataStore.PersistPath = dataPersistFile;
            string key1 = "testkey1";
            string key2 = "testkey2";
            JObject json1 = TestUtils.GenerateJson();
            JObject json2 = TestUtils.GenerateJson();
            JObject json3 = TestUtils.GenerateJson();

            dataStore.Insert(key1, json1);
            dataStore.Insert(key2, json2);

            Dictionary<string, JObject> listResult = dataStore.List();
            Assert.AreEqual(2, listResult.Count);
            Assert.IsNotNull(listResult[key1]);
            Assert.IsNotNull(listResult[key2]);

            JObject getResult = dataStore.Get(key1);
            Assert.True(JsonConvert.SerializeObject(getResult).Equals(JsonConvert.SerializeObject(json1)));


            dataStore.Insert(key2, json3);
            JObject getResult2 = dataStore.Get(key2);
            Assert.True(JsonConvert.SerializeObject(getResult2).Equals(JsonConvert.SerializeObject(json3)));

            dataStore.Save();
            Assert.True(File.Exists(dataPersistFile));

            string savedFileContent = File.ReadAllText(dataPersistFile);
            Debug.WriteLine(String.Format("Save file content = {0}", savedFileContent));
            Assert.True(savedFileContent.Length > 0);

            IDataStore<JObject> loadedDataStore = InMemoryDataStore<JObject>.Load<JObject>(dataPersistFile);
            Dictionary<string, JObject> listResult2 = loadedDataStore.List();
            Assert.AreEqual(2, listResult2.Count);
            Assert.IsNotNull(listResult2[key1]);
            Assert.IsNotNull(listResult2[key2]);

            JObject getResult3 = loadedDataStore.Get(key2);
            Assert.True(JsonConvert.SerializeObject(getResult3).Equals(JsonConvert.SerializeObject(json3)));

            loadedDataStore.Delete(key1);
            listResult2 = loadedDataStore.List();
            Assert.AreEqual(1, listResult2.Count);
            Assert.IsNull(loadedDataStore.Get(key1));
        }
    }
}

