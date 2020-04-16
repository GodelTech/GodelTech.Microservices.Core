using System;

namespace GodelTech.Microservices.Core.Services
{
    public interface IDateTimeProvider
    {
        DateTime GetUtcNow();
    }
}