using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Windows.Storage;

namespace FHSDK.Services
{
    class IOService : IIOService
    {
        public IOService()
        {

        }

        public string ReadFile(string fullPath)
        {
            string content = null;
            if (File.Exists(fullPath))
            {
                StreamReader sr = new StreamReader(fullPath);
                content = sr.ReadToEnd();
                sr.Close();
            }
            return content;
        }

        public void WriteFile(string fullPath, string content)
        {
            string parentDir = Path.GetDirectoryName(fullPath);
            Directory.CreateDirectory(parentDir);
            StreamWriter writer = File.CreateText(fullPath);
            writer.Write(content);
            writer.Close();
        }

        public bool Exists(string fullPath)
        {
            return File.Exists(fullPath);
        }

        public string GetDataPersistDir()
        {
            StorageFolder local = Windows.Storage.ApplicationData.Current.LocalFolder;
            return local.Path;
        }
    }


   

}
