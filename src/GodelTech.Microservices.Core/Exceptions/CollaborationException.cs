using System;
using System.Runtime.Serialization;

namespace GodelTech.Microservices.Core.Exceptions
{
    public class CollaborationException : Exception
    {
        public int StatusCode { get; set; }
        public string ReasonPhrase { get; set; }

        public CollaborationException()
        {
        }

        public CollaborationException(string message) 
            : base(message)
        {
        }

        public CollaborationException(string message, Exception innerException) 
            : base(message, innerException)
        {
        }

        protected CollaborationException(SerializationInfo info, StreamingContext context) 
            : base(info, context)
        {
        }
    }
}
