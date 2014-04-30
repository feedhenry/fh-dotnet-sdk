using System;

namespace FHSDK.Services
{
	public interface ILogService
	{
	    void setLogLevel (int logLevel);
		void v (string tag, string message, Exception e);
		void d (string tag, string message, Exception e);
		void i (string tag, string message, Exception e);
		void w (string tag, string message, Exception e);
		void e (string tag, string message, Exception e);
	}
}

