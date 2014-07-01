using System;

namespace FHSDK.Services
{

    public interface IMonitorService
    {
        
        int MonitorInterval { set; get; }

        Boolean IsRunning { get; }

        void StartMonitor(CheckDatasetDelegate target);

        void StopMonitor();


    }

    public delegate void CheckDatasetDelegate();


}

