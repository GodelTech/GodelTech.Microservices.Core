using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;

namespace GodelTech.Microservices.Core.Utils
{
    public static class InMemoryDictionaryExtensions
    {
        public static IConfigurationBuilder AddInMemoryCollectionFromEnvVariables(this IConfigurationBuilder configurationBuilder, IReadOnlyDictionary<string, string> envVarToKeyMapping)
        {
            if (envVarToKeyMapping == null)
                throw new ArgumentNullException(nameof(envVarToKeyMapping));

            return configurationBuilder.AddInMemoryCollection(CreateDictionary(envVarToKeyMapping));
        }

        private static Dictionary<string, string> CreateDictionary(IReadOnlyDictionary<string, string> envVarToKeyMapping)
        {
            var dictionary = new Dictionary<string, string>();

            foreach (var pair in envVarToKeyMapping)
            {
                var value = Environment.GetEnvironmentVariable(pair.Key);

                if (!string.IsNullOrEmpty(value))
                    dictionary.Add(pair.Value, value);
            }

            return dictionary;
        }
    }
}