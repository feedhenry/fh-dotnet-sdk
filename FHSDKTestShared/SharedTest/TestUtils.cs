using System;
using System.Linq;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using System.IO;

#if WINDOWS_PHONE
using Microsoft.VisualStudio.TestTools.UnitTesting;
#else
using NUnit.Framework;
#endif

namespace FHSDKTestShared
{
    public class TestUtils
    {
        public TestUtils()
        {
            
        }

        public static string RandomString(int length)
        {
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            var result = new string(
                Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)])
                .ToArray());
            return result;
        }

        public static JObject GenerateJson()
        {
            JObject json = new JObject();
            string key = TestUtils.RandomString(16);
            string value = TestUtils.RandomString(16);
            json[key] = value;
            return json;
        }

        public static void AssertFileExists(string filePath)
        {
            Debug.WriteLine(string.Format("Checking existence for file {0}", filePath));
            Assert.IsTrue(File.Exists(filePath));
            string fileContent = null;
            StreamReader reader = new StreamReader(filePath);
            fileContent = reader.ReadToEnd();
            reader.Close();
            Debug.WriteLine(string.Format("File content for {0} is {1}", filePath, fileContent));
            Assert.IsFalse(string.IsNullOrEmpty(fileContent));
        }

        public static void DeleteFileIfExists(string filePath)
        {
            if(null != filePath && File.Exists(filePath)){
                File.Delete(filePath);
            }
        }
    }
}

