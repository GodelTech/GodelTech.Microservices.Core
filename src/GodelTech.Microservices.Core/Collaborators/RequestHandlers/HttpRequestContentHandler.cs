using System.Net.Http;

namespace GodelTech.Microservices.Core.Collaborators.RequestHandlers
{
    public class HttpRequestContentHandler : IRequestContentHandler
    {
        public HttpContent Create(object content)
        {
            return (HttpContent) content;
        }
    }
}