using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FHSDK.FHHttpClient
{
    public class FHException : Exception
    {
        public enum ErrorCode
        {
            UnknownError = -1,
            NetworkError = 1,
            HttpError = 2,
            ServerError = 3

        }

        private ErrorCode errorCode = ErrorCode.UnknownError;

        public FHException()
            : base()
        {
        }

        public FHException(string message)
            : base(message)
        {
        }

        public FHException(string message, ErrorCode errorCode)
            : base(message)
        {
            this.errorCode = errorCode;
        }

        public FHException(string message, ErrorCode errorCode, Exception baseException)
            : base(message, baseException)
        {
            this.errorCode = errorCode;
        }

        public ErrorCode Error
        {
            get
            {
                return this.errorCode;
            }
        }
    }
}
