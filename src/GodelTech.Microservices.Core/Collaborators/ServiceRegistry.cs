using System;
using System.Collections.Generic;

namespace GodelTech.Microservices.Core.Collaborators
{
    public class ServiceRegistry : IServiceRegistry
    {
        private readonly Dictionary<string, IServiceConfig> _serviceConfigs;

        public ServiceRegistry(IDictionary<string, IServiceConfig> config)
        {
            if (config == null)
                throw new ArgumentNullException(nameof(config));

            _serviceConfigs = new Dictionary<string, IServiceConfig>(config, StringComparer.OrdinalIgnoreCase);
        }

        public IServiceConfig GetConfig(string serviceName)
        {
            if (string.IsNullOrWhiteSpace(serviceName))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(serviceName));

            return _serviceConfigs.ContainsKey(serviceName) ? _serviceConfigs[serviceName] : null;
        }
    }
}