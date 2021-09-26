using System;
using System.Collections.Generic;
using System.Linq;
using GodelTech.Microservices.Core.IntegrationTests.Fakes.Business.Contracts;
using GodelTech.Microservices.Core.IntegrationTests.Fakes.Models;

namespace GodelTech.Microservices.Core.IntegrationTests.Fakes.Business
{
    public class FakeService : IFakeService
    {
        private static readonly IReadOnlyList<FakeModel> Items = new List<FakeModel>
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
            return Items
                .ToList();
        }

        public FakeModel Get(int id)
        {
            return Items
                .FirstOrDefault(x => x.Id == id);
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

        public void ThrowArgumentException(string name)
        {
            throw new ArgumentException("Fake ArgumentException", nameof(name));
        }
    }
}