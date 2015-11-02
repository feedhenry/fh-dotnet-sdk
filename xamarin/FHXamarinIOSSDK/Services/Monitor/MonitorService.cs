using System;
using System.Threading;
using FHSDK.Services.Monitor;

namespace FHSDK.Services
{
    public class MonitorService : IMonitorService
    {
        Timer timer = null;
        private CheckDatasetDelegate targetDelegate;

        public MonitorService()
        {
            IsRunning = false;
        }

        public int MonitorInterval { set; get; }

        public Boolean IsRunning { get; private set;}

        public void StartMonitor(CheckDatasetDelegate target)
        {
            this.targetDelegate = target;
            if(!IsRunning){
                TimerCallback tcb = RunTarget;
                timer = new Timer(tcb, null, 0, this.MonitorInterval);
                IsRunning = true;
            }
        }

        private void RunTarget(object info)
        {
            this.targetDelegate();
        }

        public void StopMonitor()
        {
            if(null != timer){
                timer.Dispose();
                timer = null;
            }
            IsRunning = false;
        }
    }
}
