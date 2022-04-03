using System.Collections.Generic;

namespace GodelTech.Microservices.Core.IntegrationTests.Fakes.Models.Fake
{
    public class FakeModel
    {
        public int Id { get; set; }

        public string ServiceName { get; set; }

        public string Message { get; set; }

#pragma warning disable CA2227 // Collection properties should be read only
                               // You can suppress the warning if the property is part of a Data Transfer Object (DTO) class.
        public Dictionary<string, string> Dictionary { get; set; }
#pragma warning restore CA2227 // Collection properties should be read only

#pragma warning disable CA2227 // Collection properties should be read only
                               // You can suppress the warning if the property is part of a Data Transfer Object (DTO) class.
        public Dictionary<string, string> EmptyDictionary { get; set; }
#pragma warning restore CA2227 // Collection properties should be read only

        public int IntValue { get; set; }

        public int? NullableIntValue { get; set; }

        public FakeStatus Status { get; set; }
    }
}