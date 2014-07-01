using System;
using Android.Util;

namespace FHSDK.Services
{
    /// <summary>
    /// Logging service provider for Android
    /// </summary>
	public class LogService: LogServiceBase
	{
		public LogService (): base()
		{
		}

		protected override void writeLog(LogLevels level, string tag, string message, System.Exception e)
		{
            string msg = message;
            if(null != e){
                msg = e.StackTrace;
            }
			if (level == LogLevels.VERBOSE) {
                Log.Verbose(tag, msg);
			} else if (level == LogLevels.DEBUG) {
				Log.Debug (tag, msg);
			} else if (level == LogLevels.INFO) {
                Log.Info (tag, msg);
			} else if (level == LogLevels.WARNING) {
                Log.Warn (tag, msg);
			}  else if (level == LogLevels.ERROR) {
                Log.Error (tag, msg);
			}
		}

	}
}

