using System.Net.Http;
using System.Threading.Tasks;

namespace GodelTech.Microservices.Core.Collaborators.ResponseHandlers
{
    public interface IResponseHandler
    {
        Task<T> ReadContent<T>(HttpResponseMessage response);
    }
}