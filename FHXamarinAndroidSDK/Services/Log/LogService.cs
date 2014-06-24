using System;
using Android.Util;
using Java.Lang;

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
            string errorMessage = "";
            if(null != e){
                errorMessage = e.StackTrace;
            }
			if (level == LogLevels.VERBOSE) {
				Log.Verbose (tag, errorMessage, message);
			} else if (level == LogLevels.DEBUG) {
				Log.Debug (tag, errorMessage, message);
			} else if (level == LogLevels.INFO) {
                Log.Info (tag, errorMessage, message);
			} else if (level == LogLevels.WARNING) {
                Log.Warn (tag, errorMessage, message);
			}  else if (level == LogLevels.ERROR) {
                Log.Error (tag, errorMessage, message);
			}
		}

	}
}

