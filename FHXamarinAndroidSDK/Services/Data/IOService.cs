using System;
using Android.App;
using System.IO;
using FHSDK.Services.Data;
using FHSDK.Services.Log;

namespace FHSDK.Services
{
    public class IOService : IIOService
    {
        private const string TAG = "FHSDK.IOService";
        ILogService logger = ServiceFinder.Resolve<ILogService>();
        public IOService()
        {
        }

        public string ReadFile(string fullPath)
        {
            string content = null;
            Java.IO.File file = new Java.IO.File(fullPath);
            if(file.Exists()){
                StreamReader sr = new StreamReader(file.AbsolutePath, System.Text.Encoding.UTF8);
                content = sr.ReadToEnd();
                sr.Close();
            } 
            return content;
        }

        public void WriteFile(string fullPath, string content)
        {
            
            Java.IO.File file = new Java.IO.File(fullPath);
            if(!file.Exists()){
                Java.IO.File parentDir = file.ParentFile;
                if(!parentDir.Exists()) {
                    parentDir.Mkdirs();
                }
                file.CreateNewFile();
            }

            StreamWriter writer = new StreamWriter(file.AbsolutePath);
            writer.Write(content);
            writer.Close();
        }

        public bool Exists(string fullPath)
        {
            Java.IO.File file = new Java.IO.File(fullPath);
            return file.Exists();
        }

        public string GetDataPersistDir()
        {
            Java.IO.File dataDir = Application.Context.GetExternalFilesDir(Android.OS.Environment.DirectoryDownloads);
            if(null == dataDir){
                dataDir = new Java.IO.File(Environment.GetFolderPath(Environment.SpecialFolder.Personal));
            }
            return dataDir.AbsolutePath;
        }
    }
}

