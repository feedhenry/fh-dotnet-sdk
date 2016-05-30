using System;

namespace FHSDK.Services
{
    /// <summary>
    /// Logging service provider for iOS
    /// </summary>
	public class LogService: LogServiceBase
	{
		public LogService (): base()
		{
		}

		protected override void writeLog(LogLevels level, string tag, string message, System.Exception e)
		{
			string output = String.Format ("[{0}]:[{1}] - {2} - {3}", level, tag, message, null == e ? "" : e.StackTrace);
			Console.WriteLine (output);
		}
	}
}

