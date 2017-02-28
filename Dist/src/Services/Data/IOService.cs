using System;
using System.IO;
using FHSDK.Services.Data;

namespace FHSDK.Services
{
    public class IOService : IIOService
    {
        public IOService()
        {
        }

        public string ReadFile(string fullPath)
        {
            string content = null;
            if(File.Exists(fullPath)){
                StreamReader sr = new StreamReader(fullPath);
                content = sr.ReadToEnd();
                sr.Close();
            }  
            return content;
        }

        public void WriteFile(string fullPath, string content)
        {
            string parentDir = Path.GetDirectoryName(fullPath);
            if(!Directory.Exists(parentDir)){
                Directory.CreateDirectory(parentDir);
            }
            File.WriteAllText(fullPath, content);
        }

        public bool Exists(string fullPath)
        {
            return File.Exists(fullPath);
        }

        public string GetDataPersistDir()
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        }


    }
}

