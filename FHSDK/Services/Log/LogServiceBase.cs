using System;
using FHSDK.Services.Log;

namespace FHSDK.Services
{
	public abstract class LogServiceBase : ILogService
	{
		public enum LogLevels: int {VERBOSE=1, DEBUG=2, INFO=3, WARNING=4, ERROR=5, NONE=Int16.MaxValue}
		protected int logLevel = (int) LogLevels.NONE;

		public LogServiceBase ()
		{
		}

		public void SetLogLevel (int level)
		{
			logLevel = level;
		}

		public void v (string tag, string message, Exception e)
		{
			doLog (LogLevels.VERBOSE, tag, message, e);
		}

		public void d (string tag, string message, Exception e)
		{
			doLog (LogLevels.DEBUG, tag, message, e);
		}

		public void i (string tag, string message, Exception e)
		{
			doLog (LogLevels.INFO, tag, message, e);
		}

		public void w (string tag, string message, Exception e)
		{
			doLog (LogLevels.WARNING, tag, message, e);
		}

		public void e (string tag, string message, Exception e)
		{
			doLog (LogLevels.ERROR, tag, message, e);
		}

		protected void doLog(LogLevels level, string tag, string message, Exception e)
		{
			if ((int)level >= logLevel) {
				writeLog (level, tag, message, e);
			}
		}

		protected abstract void writeLog(LogLevels level, string tag, string message, Exception e);

	}
}

