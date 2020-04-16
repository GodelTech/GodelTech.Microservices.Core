using System.Collections.Generic;
using System.Linq;

namespace GodelTech.Microservices.Core.Collaborators
{
    public class ServiceConfigSection
    {
        public Dictionary<string, ServiceConfig> ServiceEndpoints { get; set; }

        public IDictionary<string, IServiceConfig> GetServices()
        {
            return ServiceEndpoints?.ToDictionary(x => x.Key, x => (IServiceConfig) x.Value) ?? new Dictionary<string, IServiceConfig>();
        }
    }
}