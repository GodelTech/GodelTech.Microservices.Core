using GodelTech.Microservices.Core.IntegrationTests.Fakes.Contracts;
using GodelTech.Microservices.Core.IntegrationTests.Fakes.Models;
using System.Collections.Generic;
using System.Linq;

namespace GodelTech.Microservices.Core.IntegrationTests.Fakes
{
    public class FakeService : IFakeService
    {
        private readonly IReadOnlyList<FakeModel> _items = new List<FakeModel>
        {
            new FakeModel(),
            new FakeModel
            {
                Id = 1,
                ServiceName = nameof(FakeService),
                Message = "Test Message",
                Dictionary = new Dictionary<string, string>
                {
                    { "FirstKey", "FirstValue" },
                    { "Second Key", "Second Value" },
                    { "third key", "third value" }
                },
                IntValue = 97,
                NullableIntValue = null,
                Status = FakeStatus.Default
            },
            new FakeModel
            {
                Id = 2,
                IntValue = 97,
                NullableIntValue = 3,
                Status = FakeStatus.Other
            }
        };
        
        public IList<FakeModel> GetList()
        {
            return _items
                .ToList();
        }

        public void ThrowFileTooLargeException()
        {
            throw new FileTooLargeException();
        }

        public void ThrowRequestValidationException()
        {
            throw new RequestValidationException();
        }

        public void ThrowResourceNotFoundException()
        {
            throw new ResourceNotFoundException();
        }
    }
}