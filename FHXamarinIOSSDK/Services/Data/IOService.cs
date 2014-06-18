using System;
using System.IO;

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
            }  
            return content;
        }

        public void WriteFile(string fullPath, string content)
        {
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

