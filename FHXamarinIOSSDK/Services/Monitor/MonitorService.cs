using System;
using System.Threading;

namespace FHSDK.Services
{
    public class MonitorService : IMonitorService
    {
        Timer timer = null;

        public MonitorService()
        {
            IsRunning = false;
        }

        public int MonitorInterval { set; get; }

        public Boolean IsRunning { get; private set;}

        public void StartMonitor(Delegate target)
        {
            if(!IsRunning){
                TimerCallback tcb = (TimerCallback) target;
                timer = new Timer(tcb, null, 0, this.MonitorInterval);
                IsRunning = true;
            }
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
