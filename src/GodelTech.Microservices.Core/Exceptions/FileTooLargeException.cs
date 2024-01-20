using System;

namespace GodelTech.Microservices.Core
{
    /// <summary>
    /// Is used to indicate that file too large exception occurs.
    /// </summary>
    public class FileTooLargeException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FileTooLargeException"/> class.
        /// </summary>
        public FileTooLargeException()
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FileTooLargeException"/> class.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        public FileTooLargeException(string message)
            : base(message)
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FileTooLargeException"/> class.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference (<see langword="Nothing" /> in Visual Basic) if no inner exception is specified.</param>
        public FileTooLargeException(string message, Exception innerException)
            : base(message, innerException)
        {

        }
    }
}
