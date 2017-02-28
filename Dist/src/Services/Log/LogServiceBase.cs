using System;
using FHSDK.Services.Log;

namespace FHSDK.Services
{
    /// <summary>
    /// Abstract based implementation of ILogService.
    /// Provide a service interface provides logging.
    /// </summary>
    public abstract class LogServiceBase : ILogService
	{
        /// <summary>
        /// Deifferent log levels.
        /// </summary>
        public enum LogLevels: int {VERBOSE=1, DEBUG=2, INFO=3, WARNING=4, ERROR=5, NONE=Int16.MaxValue}
        
        /// <summary>
        /// Actual log level.
        /// </summary>
        protected int logLevel = (int) LogLevels.NONE;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public LogServiceBase ()
		{
		}

        /// <summary>
        /// Set log level.
        /// </summary>
        /// <param name="level"></param>
		public void SetLogLevel (int level)
		{
			logLevel = level;
		}

        /// <summary>
        /// Do verbose logging.
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="message"></param>
        /// <param name="e"></param>
		public void v (string tag, string message, Exception e)
		{
			doLog (LogLevels.VERBOSE, tag, message, e);
		}

        /// <summary>
        /// Do debug logging.
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="message"></param>
        /// <param name="e"></param>
		public void d (string tag, string message, Exception e)
		{
			doLog (LogLevels.DEBUG, tag, message, e);
		}

        /// <summary>
        /// Do info logging.
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="message"></param>
        /// <param name="e"></param>
		public void i (string tag, string message, Exception e)
		{
			doLog (LogLevels.INFO, tag, message, e);
		}

        /// <summary>
        /// Do warning logging.
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="message"></param>
        /// <param name="e"></param>
		public void w (string tag, string message, Exception e)
		{
			doLog (LogLevels.WARNING, tag, message, e);
		}

        /// <summary>
        /// Do error logging.
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="message"></param>
        /// <param name="e"></param>
		public void e (string tag, string message, Exception e)
		{
			doLog (LogLevels.ERROR, tag, message, e);
		}

        /// <summary>
        /// Method that writes log infor according to log level.
        /// </summary>
        /// <param name="level"></param>
        /// <param name="tag"></param>
        /// <param name="message"></param>
        /// <param name="e"></param>
		protected void doLog(LogLevels level, string tag, string message, Exception e)
		{
			if ((int)level >= logLevel) {
				writeLog (level, tag, message, e);
			}
		}

        /// <summary>
        /// Write log to chosen file/console log mechanism.
        /// </summary>
        /// <param name="level"></param>
        /// <param name="tag"></param>
        /// <param name="message"></param>
        /// <param name="e"></param>
		protected abstract void writeLog(LogLevels level, string tag, string message, Exception e);

	}
}

