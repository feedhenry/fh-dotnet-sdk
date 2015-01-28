using FHSDK.Services;
using System;
using System.Diagnostics;

namespace FHSDK81.Services
{
    /// <summary>
    /// Logging service for windows phone
    /// </summary>
    class LogService: LogServiceBase
    {
        public LogService()
            : base()
        {
        }

        protected override void writeLog(LogServiceBase.LogLevels level, string tag, string message, Exception e)
        {
            string output = string.Format("[{0}]:[{1}] - {2} - {3}", level, tag, message, null == e ? "" : e.StackTrace);
            Debug.WriteLine(output);
        }
    }
}
