#if __ANDROID__
using FHSDK.Droid;
#elif __IOS__
using FHSDK.Touch;
#endif
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using FHSDK;
using FHSDK.Sync;
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
    public class FHSyncClientTest
    {
        private const string DATASET_ID = "datasetclient_tasks";
        private string dataFilePath;
        private string metaDataFilePath;
        private string pendingFilePath;
        private FHSyncClient syncClient;

        [SetUp]
        public void SetUp()
        {
            FHClient.Init();
            FH.SetLogLevel(1);
        }

        [TearDown]
        public void TearDown()
        {
            TestUtils.DeleteFileIfExists(metaDataFilePath);
            TestUtils.DeleteFileIfExists(dataFilePath);
            TestUtils.DeleteFileIfExists(pendingFilePath);
            if (null != syncClient)
            {
                Debug.WriteLine("Stop running syncing client");
                syncClient.StopAll();
            }
        }

        [Test]
#if WINDOWS_PHONE
        public async Task TestFHSyncClient()
#else 
        public async void TestFHSyncClient()
        #endif
        {
            //clear db
            var setupRes = await FH.Cloud(string.Format("/syncTest/{0}", DATASET_ID), "DELETE", null, null);
            Assert.IsTrue(HttpStatusCode.OK.Equals(setupRes.StatusCode));

            var syncConfig = new FHSyncConfig();
            syncConfig.SyncActive = false;
            syncConfig.SyncFrequency = 1;
            syncConfig.AutoSyncLocalUpdates = true;
            syncConfig.SyncCloud = FHSyncConfig.SyncCloudType.Mbbas;

            //make sure no existing data file exist
            metaDataFilePath = FHSyncUtils.GetDataFilePath(DATASET_ID, ".sync.json");
            dataFilePath = FHSyncUtils.GetDataFilePath(DATASET_ID, ".data.json");
            pendingFilePath = FHSyncUtils.GetDataFilePath(DATASET_ID, ".pendings.json");

            TestUtils.DeleteFileIfExists(metaDataFilePath);
            TestUtils.DeleteFileIfExists(dataFilePath);
            TestUtils.DeleteFileIfExists(pendingFilePath);

            Assert.IsFalse(File.Exists(metaDataFilePath));
            Assert.IsFalse(File.Exists(dataFilePath));
            Assert.IsFalse(File.Exists(pendingFilePath));

            syncClient = FHSyncClient.GetInstance();
            syncClient.Initialise(syncConfig);
            syncClient.Manage<TaskModel>(DATASET_ID, syncConfig, null);

            var syncStarted = false;
            var syncCompleted = false;

            syncClient.SyncStarted += (object sender, FHSyncNotificationEventArgs e) =>
            {
                if (e.DatasetId.Equals(DATASET_ID))
                {
                    syncStarted = true;
                }
            };

            syncClient.SyncCompleted += (object sender, FHSyncNotificationEventArgs e) =>
            {
                if (e.DatasetId.Equals(DATASET_ID))
                {
                    syncCompleted = true;
                }
            };

            var tasks = syncClient.List<TaskModel>(DATASET_ID);
            Assert.AreEqual(0, tasks.Count);

            var newTask = new TaskModel
            {
                TaksName = "task1",
                Completed = false
            };
            newTask = syncClient.Create(DATASET_ID, newTask);
            //syncConfig.SyncActive = true;
            Assert.IsNotNull(newTask.UID);

            syncClient.ForceSync<TaskModel>(DATASET_ID);

            Thread.Sleep(2000);

            var cloudRes = await FH.Cloud(string.Format("/syncTest/{0}", DATASET_ID), "GET", null, null);
            Assert.IsNull(cloudRes.Error);
            var dbData = cloudRes.GetResponseAsJObject();
            Assert.AreEqual(1, (int) dbData["count"]);
            var taskNameInDb = (string) dbData["list"][0]["fields"]["taskName"];
            Assert.IsTrue(taskNameInDb.Equals("task1"));

            Assert.IsTrue(syncStarted);
            Assert.IsTrue(syncCompleted);

            syncClient.StopAll();
        }
    }
}