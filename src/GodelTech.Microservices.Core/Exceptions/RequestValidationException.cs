using System;
using System.Runtime.Serialization;

namespace GodelTech.Microservices.Core
{
    /// <summary>
    /// Is used to indicate that request validation exception occurs.
    /// </summary>
    [Serializable]
    public class RequestValidationException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RequestValidationException"/> class.
        /// </summary>
        public RequestValidationException()
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestValidationException"/> class.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        public RequestValidationException(string message)
            : base(message)
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestValidationException"/> class.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference (<see langword="Nothing" /> in Visual Basic) if no inner exception is specified.</param>
        public RequestValidationException(string message, Exception innerException)
            : base(message, innerException)
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Exception"></see> class with serialized data.
        /// </summary>
        /// <param name="info">The <see cref="SerializationInfo"></see>.</param>
        /// <param name="context">The <see cref="StreamingContext"></see>.</param>
        protected RequestValidationException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {

        }
    }
}
