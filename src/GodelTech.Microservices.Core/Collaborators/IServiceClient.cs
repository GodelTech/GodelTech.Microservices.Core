using System;
using System.Threading.Tasks;

namespace GodelTech.Microservices.Core.Collaborators
{
    public interface IServiceClient : IDisposable
    {
        Task<TResult> HeadAsync<TResult>(string url)
            where TResult : class;

        Task<TResult> GetAsync<TResult>(string url)
            where TResult : class;

        Task<TResult> PostAsync<TResult>(string url, object body = null)
            where TResult : class;

        Task<TResult> PutAsync<TResult>(string url, object body = null)
            where TResult : class;

        Task<TResult> DeleteAsync<TResult>(string url, object body = null)
            where TResult : class;
    }
}