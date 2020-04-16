using System.Threading.Tasks;

namespace GodelTech.Microservices.Core.Services
{
    public interface IIdentityService
    {
        Task<string> GetClientCredentialsTokenAsync();
        Task<string> GetTenantClientCredentialsTokenAsync(int tenantId);
    }
}