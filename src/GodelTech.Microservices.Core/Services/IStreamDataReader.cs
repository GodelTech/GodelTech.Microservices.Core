using System.IO;
using System.Threading.Tasks;

namespace GodelTech.Microservices.Core.Services
{
    public interface IStreamDataReader
    {
        Task<byte[]> ReadFromStreamAsync(Stream stream, int maxFileSize);
    }
}