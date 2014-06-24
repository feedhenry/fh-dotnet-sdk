using System;
using System.Linq;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using NUnit.Framework;
using System.IO;

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
            Assert.True(File.Exists(filePath));
            string fileContent = File.ReadAllText(filePath);
            Debug.WriteLine(string.Format("File content for {0} is {1}", filePath, fileContent));
            Assert.False(string.IsNullOrEmpty(fileContent));
        }

        public static void DeleteFileIfExists(string filePath)
        {
            if(null != filePath && File.Exists(filePath)){
                File.Delete(filePath);
            }
        }
    }
}

