using System;

namespace GodelTech.Microservices.Core.Services
{
    public interface IGuidFactory
    {
        Guid New();
        string NewAsString();
    }
}