using System;

namespace GodelTech.Microservices.Core.Collaborators
{
    public class ServiceConfig : IServiceConfig
    {
        public bool ExcludeAccessToken { get; set; }
        public string BaseAddress { get; set; }
        public TimeSpan Timeout { get; set; } = new TimeSpan(0, 0, 0, 30);
    }
}