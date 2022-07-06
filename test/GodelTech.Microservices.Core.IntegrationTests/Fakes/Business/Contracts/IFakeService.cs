using System.Collections.Generic;
using System.Threading.Tasks;
using GodelTech.Microservices.Core.IntegrationTests.Fakes.Business.Models;

namespace GodelTech.Microservices.Core.IntegrationTests.Fakes.Business.Contracts
{
    public interface IFakeService
    {
        IList<FakeDto> GetList();

        FakeDto Get(int id);

        FakeDto Add(IFakeAddDto item);

        Task CompleteAsync();

        void ThrowFileTooLargeException();

        void ThrowRequestValidationException();

        void ThrowResourceNotFoundException();

        void ThrowArgumentException(string name);
    }
}
