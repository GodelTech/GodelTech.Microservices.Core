using System.Collections.Generic;
using GodelTech.Microservices.Core.IntegrationTests.Fakes.Models;

namespace GodelTech.Microservices.Core.IntegrationTests.Fakes.Contracts
{
    public interface IFakeService
    {
        IList<FakeModel> GetList();

        void ThrowFileTooLargeException();

        void ThrowRequestValidationException();

        void ThrowResourceNotFoundException();

        void ThrowArgumentException(string name);
    }
}