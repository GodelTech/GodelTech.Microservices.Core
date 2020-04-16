using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace GodelTech.Microservices.Core.Services
{
    public class JsonSerializer : IJsonSerializer
    {
        private static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            NullValueHandling = NullValueHandling.Ignore,
            Formatting = Formatting.Indented
            
        };

        public T Deserialize<T>(string content)
        {
            if (string.IsNullOrWhiteSpace(content))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(content));

            return JsonConvert.DeserializeObject<T>(content, Settings);
        }

        public string Serialize(object data)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));

            return JsonConvert.SerializeObject(data, Settings);
        }
    }
}
