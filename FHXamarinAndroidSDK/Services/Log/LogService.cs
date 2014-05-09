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
			if (level == LogLevels.VERBOSE) {
				Log.Verbose (tag, (Throwable)e, message);
			} else if (level == LogLevels.DEBUG) {
				Log.Debug (tag, (Throwable)e, message);
			} else if (level == LogLevels.INFO) {
				Log.Info (tag, (Throwable)e, message);
			} else if (level == LogLevels.WARNING) {
				Log.Warn (tag, (Throwable)e, message);
			}  else if (level == LogLevels.ERROR) {
				Log.Error (tag, (Throwable)e, message);
			}
		}

	}
}

