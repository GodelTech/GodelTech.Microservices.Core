using System.Collections.Generic;
using System.Threading.Tasks;
using GodelTech.Microservices.Core.IntegrationTests.Fakes.Business.Models;

namespace GodelTech.Microservices.Core.IntegrationTests.Fakes.Business.Contracts
{
    public interface IFakeService
    {
        IList<FakeDto> GetList();

#pragma warning disable CA1716 // Identifiers should not match keywords
        FakeDto Get(int id);
#pragma warning restore CA1716 // Identifiers should not match keywords

        FakeDto Add(IFakeAddDto item);

        Task CompleteAsync();

        void ThrowFileTooLargeException();

        void ThrowRequestValidationException();

        void ThrowResourceNotFoundException();

        void ThrowArgumentException(string name);
    }
}
