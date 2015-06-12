using System.IO;
using Windows.Storage;
using FHSDK.Services.Data;

namespace FHSDK.Services
{
    internal class IOService : IIOService
    {
        public string ReadFile(string fullPath)
        {
            if (!File.Exists(fullPath)) return null;
            var sr = new StreamReader(fullPath);
            var content = sr.ReadToEnd();
            sr.Close();
            return content;
        }

        public void WriteFile(string fullPath, string content)
        {
            var parentDir = Path.GetDirectoryName(fullPath);
            Directory.CreateDirectory(parentDir);
            var writer = File.CreateText(fullPath);
            writer.Write(content);
            writer.Close();
        }

        public bool Exists(string fullPath)
        {
            return File.Exists(fullPath);
        }

        public string GetDataPersistDir()
        {
            var local = ApplicationData.Current.LocalFolder;
            return local.Path;
        }
    }
}