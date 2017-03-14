using System.Threading;

namespace FHSDK.Services.Monitor
{
    public class MonitorService : IMonitorService
    {
        private CheckDatasetDelegate targetDelegate;
        private Timer timer;

        public MonitorService()
        {
            IsRunning = false;
        }

        public int MonitorInterval { set; get; }
        public bool IsRunning { get; private set; }

        public void StartMonitor(CheckDatasetDelegate target)
        {
            targetDelegate = target;
            if (IsRunning) return;
            TimerCallback tcb = RunTarget;
            timer = new Timer(tcb, null, 0, MonitorInterval);
            IsRunning = true;
        }

        public void StopMonitor()
        {
            if (null != timer)
            {
                timer.Dispose();
                timer = null;
            }
            IsRunning = false;
        }

        private void RunTarget(object info)
        {
            targetDelegate();
        }
    }
}