using System;

namespace FHSDK.FHHttpClient
{
    /// <summary>
    ///     The exception that is thrown by FH API calls
    /// </summary>
    public class FHException : Exception
    {
        /// <summary>
        ///     Possible error codes
        /// </summary>
        public enum ErrorCode
        {
            /// <summary>
            ///     This error code means the error is unknown
            /// </summary>
            UnknownError = -1,

            /// <summary>
            ///     This error code means the device is offline
            /// </summary>
            NetworkError = 1,

            /// <summary>
            ///     This error code means http exception is thrown by the http client
            /// </summary>
            HttpError = 2,

            /// <summary>
            ///     This error code means the server returns error
            /// </summary>
            ServerError = 3,

            /// <summary>
            ///     This error code means authentication failed
            /// </summary>
            AuthenticationError = 4,

            /// <summary>
            ///     User cancelled
            /// </summary>
            Cancelled = 5
        }

        private readonly ErrorCode errorCode = ErrorCode.UnknownError;

        /// <summary>
        /// </summary>
        public FHException()
        {
        }

        /// <summary>
        /// </summary>
        /// <param name="message"></param>
        public FHException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// </summary>
        /// <param name="message"></param>
        /// <param name="errorCode"></param>
        public FHException(string message, ErrorCode errorCode)
            : base(message)
        {
            this.errorCode = errorCode;
        }

        /// <summary>
        /// </summary>
        /// <param name="message"></param>
        /// <param name="errorCode"></param>
        /// <param name="baseException"></param>
        public FHException(string message, ErrorCode errorCode, Exception baseException)
            : base(message, baseException)
        {
            this.errorCode = errorCode;
        }

        /// <summary>
        ///     Get the error code
        /// </summary>
        public ErrorCode Error
        {
            get { return errorCode; }
        }
    }
}