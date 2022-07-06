using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GodelTech.Microservices.Core.IntegrationTests.Fakes.Business.Contracts;
using GodelTech.Microservices.Core.IntegrationTests.Fakes.Business.Models;

namespace GodelTech.Microservices.Core.IntegrationTests.Fakes.Business
{
    public class FakeService : IFakeService
    {
        private static readonly IReadOnlyList<FakeDto> Items = new List<FakeDto>
        {
            new FakeDto(),
            new FakeDto
            {
                Id = 1,
                ServiceName = nameof(FakeService),
                Message = "Test Message",
                Dictionary = new Dictionary<string, string>
                {
                    { "FirstKey", "FirstValue" },
                    { "Second Key", "Second Value" },
                    { "third key lowercase", "third value lowercase" }
                },
                IntValue = 97,
                NullableIntValue = null,
                Status = FakeStatus.Default
            },
            new FakeDto
            {
                Id = 2,
                IntValue = 97,
                NullableIntValue = 3,
                Status = FakeStatus.Other
            }
        };

        public IList<FakeDto> GetList()
        {
            return Items
                .ToList();
        }

        public FakeDto Get(int id)
        {
            return Items
                .FirstOrDefault(x => x.Id == id);
        }

        public FakeDto Add(IFakeAddDto item)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));

            return new FakeDto
            {
                Id = 3,
                ServiceName = item.ServiceName,
                Message = item.Message
            };
        }

        public Task CompleteAsync()
        {
            return Task.CompletedTask;
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
