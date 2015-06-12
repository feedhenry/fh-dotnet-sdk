using System;
using System.Diagnostics;

namespace FHSDK.Services.Log
{
    /// <summary>
    ///     Logging service for windows phone
    /// </summary>
    internal class LogService : LogServiceBase
    {
        protected override void writeLog(LogLevels level, string tag, string message, Exception e)
        {
            var output = string.Format("[{0}]:[{1}] - {2} - {3}", level, tag, message, null == e ? "" : e.StackTrace);
            Debug.WriteLine(output);
        }
    }
}