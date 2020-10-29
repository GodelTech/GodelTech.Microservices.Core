using System;

namespace GodelTech.Microservices.Core.Exceptions
{
    public class ResourceNotFoundException : Exception
    {
        public ResourceNotFoundException(string message)
            : base(message)
        {

        }
    }
}