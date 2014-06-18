using System;

namespace FHSDK.Services
{

    public interface IMonitorService
    {
        
        int MonitorInterval { set; get; }

        Boolean IsRunning { get; }

        void StartMonitor(Delegate target);

        void StopMonitor();
    }


}

