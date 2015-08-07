using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Newtonsoft.Json.Linq;
#if WINDOWS_PHONE
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;

#else
using NUnit.Framework;
#endif

namespace FHSDKTestShared
{
    public class TestUtils
    {
        private static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            var result = new string(
                Enumerable.Repeat(chars, length)
                    .Select(s => s[random.Next(s.Length)])
                    .ToArray());
            return result;
        }

        public static JObject GenerateJson()
        {
            var json = new JObject();
            var key = RandomString(16);
            var value = RandomString(16);
            json[key] = value;
            return json;
        }

        public static void AssertFileExists(string filePath)
        {
            Debug.WriteLine("Checking existence for file {0}", filePath);
            Assert.IsTrue(File.Exists(filePath));
            string fileContent = null;
            var reader = new StreamReader(filePath);
            fileContent = reader.ReadToEnd();
            reader.Close();
            Debug.WriteLine("File content for {0} is {1}", filePath, fileContent);
            Assert.IsFalse(string.IsNullOrEmpty(fileContent));
        }

        public static void DeleteFileIfExists(string filePath)
        {
            if (null != filePath && File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }
    }
}