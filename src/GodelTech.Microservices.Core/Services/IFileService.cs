using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace GodelTech.Microservices.Core.Services
{
    public interface IFileService
    {
        Task<string> ReadAllTextAsync(string filePath);
        bool Exists(string path);
        void Delete(string path);
        Task WriteAllTextAsync(string filePath, string content);
        IEnumerable<string> FindAll(string path, string mask);
        Task<byte[]> ReadAllBytesAsync(string getSourceFilePath);
        FileStream OpenRead(string filePath);
        FileStream OpenWrite(string filePath);
    }
}