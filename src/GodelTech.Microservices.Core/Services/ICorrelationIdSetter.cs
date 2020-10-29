using System;

namespace GodelTech.Microservices.Core.Services
{
    public interface ICorrelationIdSetter
    {
        IDisposable SetCorrelationId(string id);
    }
}
