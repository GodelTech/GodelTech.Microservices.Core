using System;

namespace GodelTech.Microservices.Core.Exceptions
{
    public class RequestValidationException : Exception
    {
        public string PropertyName { get; }

        public RequestValidationException(string propertyName, string errorMessage)
            : base(errorMessage)
        {
            if (string.IsNullOrEmpty(propertyName))
                throw new ArgumentNullException(nameof(propertyName));

            PropertyName = propertyName;
        }
    }
}
