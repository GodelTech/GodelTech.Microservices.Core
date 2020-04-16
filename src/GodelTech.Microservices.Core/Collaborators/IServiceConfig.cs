using System;

namespace GodelTech.Microservices.Core.Collaborators
{
    public interface IServiceConfig
    {
        string BaseAddress { get; }
        TimeSpan Timeout { get; }
        bool ExcludeAccessToken { get; }
    }
}
