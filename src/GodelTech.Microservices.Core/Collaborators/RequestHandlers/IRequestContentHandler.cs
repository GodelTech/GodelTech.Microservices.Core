using System.Net.Http;

namespace GodelTech.Microservices.Core.Collaborators.RequestHandlers
{
    public interface IRequestContentHandler
    {
        HttpContent Create(object content);
    }
}
