using System;

namespace FHSDK.Services.Log
{
    /// <summary>
    ///     A service interface provides logging
    /// </summary>
    public interface ILogService
    {
        /// <summary>
        ///     Set the log level
        /// </summary>
        /// <param name="logLevel">log level</param>
        void SetLogLevel(int logLevel);

        /// <summary>
        ///     Do verbose logging
        /// </summary>
        /// <param name="tag">log tag</param>
        /// <param name="message">log message</param>
        /// <param name="e">exception</param>
        void v(string tag, string message, Exception e);

        /// <summary>
        ///     Do debug logging
        /// </summary>
        /// <param name="tag">log tag</param>
        /// <param name="message">log message</param>
        /// <param name="e">exception</param>
        void d(string tag, string message, Exception e);

        /// <summary>
        ///     Do info logging
        /// </summary>
        /// <param name="tag">log tag</param>
        /// <param name="message">log message</param>
        /// <param name="e">exception</param>
        void i(string tag, string message, Exception e);

        /// <summary>
        ///     Do warning logging
        /// </summary>
        /// <param name="tag">log tag</param>
        /// <param name="message">log message</param>
        /// <param name="e">exception</param>
        void w(string tag, string message, Exception e);

        /// <summary>
        ///     Do error logging
        /// </summary>
        /// <param name="tag">log tag</param>
        /// <param name="message">log message</param>
        /// <param name="e">exception</param>
        void e(string tag, string message, Exception e);
    }
}