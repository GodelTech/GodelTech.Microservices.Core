using System.Collections.Generic;
using GodelTech.Microservices.Core.IntegrationTests.Fakes.Models;

namespace GodelTech.Microservices.Core.IntegrationTests.Fakes.Business.Contracts
{
    public interface IFakeService
    {
        IList<FakeModel> GetList();

        FakeModel Get(int id);

        void ThrowFileTooLargeException();

        void ThrowRequestValidationException();

        void ThrowResourceNotFoundException();

        void ThrowArgumentException(string name);
    }
}