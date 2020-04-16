using System;
using System.Runtime.Serialization;

namespace GodelTech.Microservices.Core.Exceptions
{
    public class FileTooLargeExceptionException : Exception
    {
        public FileTooLargeExceptionException()
        {
        }

        public FileTooLargeExceptionException(string message) 
            : base(message)
        {
        }

        public FileTooLargeExceptionException(string message, Exception innerException) 
            : base(message, innerException)
        {
        }

        protected FileTooLargeExceptionException(SerializationInfo info, StreamingContext context) 
            : base(info, context)
        {
        }
    }
}