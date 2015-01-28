using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FHSDK.Services
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
