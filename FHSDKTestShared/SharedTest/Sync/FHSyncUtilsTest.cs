using NUnit.Framework;
using System;
using FHSDK.Sync;
using System.Diagnostics;
using FHSDK.Services;
using Newtonsoft.Json.Linq;


#if __ANDROID__
using FHSDK.Droid;
#elif __IOS__
using FHSDK.Touch;
#endif

namespace FHSDKTestShared
{
    //To make sure the hash functions on C# will generate the same value as other platforms
    [TestFixture]
    public class FHSyncUtilsTest
    {
        [SetUp]
        public void SetUp()
        {
            FHClient.Init();

        }

        [Test]
        public void TestStringHash()
        {
            
            string text = "test";
            IHashService hasher = ServiceFinder.Resolve<IHashService> ();
            string nativeHashed = hasher.GenerateSHA1Hash(text);
            Debug.WriteLine(string.Format("native hashed value = {0}", nativeHashed));
            string expected = "a94a8fe5ccb19ba61c4c0873d391e987982fbbd3";
            Debug.WriteLine(String.Format("got hash value {0}", nativeHashed));
            Assert.True(expected.Equals(nativeHashed));
        }

        [Test]
        public void TestObjectHash()
        {
            JObject testObject = new JObject();
            testObject["testKey"] = "Test Data";
            testObject["testBoolKey"] = true;
            testObject["testNumKey"] = 10;
            JArray arr = new JArray();
            arr.Add("obj1");
            arr.Add("obj2");
            testObject["testArrayKey"] = arr;
            JObject obj = new JObject();
            obj["obj3key"] = "obj3";
            obj["obj4key"] = "obj4";
            testObject["testDictKey"] = obj;
            string hash = FHSyncUtils.GenerateSHA1Hash(testObject);
            Debug.WriteLine(String.Format("Got hash value = {0}", hash));
            string expected = "5f4675723d658919ede35fac62fade8c6397df1d";
            Assert.True(expected.Equals(hash));
        }
    }
}

