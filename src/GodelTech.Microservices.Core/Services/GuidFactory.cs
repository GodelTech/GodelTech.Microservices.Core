using System;

namespace GodelTech.Microservices.Core.Services
{
    public class GuidFactory : IGuidFactory
    {
        public Guid New()
        {
            return Guid.NewGuid();
        }

        public string NewAsString()
        {
            return New().ToString();
        }
    }
}