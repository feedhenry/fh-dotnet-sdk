using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FHSDK.Services
{
    public interface IDeviceService
    {
        string GetDeviceId();
        void SaveData(string dataId, string dataValue);
        string GetData(string dataId);
        Task<string> ReadResourceAsString(string resourceName); 
    }
}
