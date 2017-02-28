namespace FHSDK.Services.Monitor
{
    public interface IMonitorService
    {
        int MonitorInterval { set; get; }
        bool IsRunning { get; }
        void StartMonitor(CheckDatasetDelegate target);
        void StopMonitor();
    }

    public delegate void CheckDatasetDelegate();
}