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
				Android.Util.Log.Verbose(tag, msg);
			} else if (level == LogLevels.DEBUG) {
				Android.Util.Log.Debug (tag, msg);
			} else if (level == LogLevels.INFO) {
				Android.Util.Log.Info (tag, msg);
			} else if (level == LogLevels.WARNING) {
				Android.Util.Log.Warn (tag, msg);
			}  else if (level == LogLevels.ERROR) {
				Android.Util.Log.Error (tag, msg);
			}
		}

	}
}

