using System.Threading;

namespace FHSDK.Services.Monitor
{
    public class MonitorService : IMonitorService
    {
        private CheckDatasetDelegate _targetDelegate;
        private Timer _timer;

        public MonitorService()
        {
            IsRunning = false;
        }

        public int MonitorInterval { set; get; }
        public bool IsRunning { get; private set; }

        public void StartMonitor(CheckDatasetDelegate target)
        {
            _targetDelegate = target;
            if (IsRunning) return;
            TimerCallback tcb = RunTarget;
            _timer = new Timer(tcb, null, 0, MonitorInterval);
            IsRunning = true;
        }

        public void StopMonitor()
        {
            if (null != _timer)
            {
                _timer.Dispose();
                _timer = null;
            }
            IsRunning = false;
        }

        private void RunTarget(object info)
        {
            _targetDelegate();
        }
    }
}