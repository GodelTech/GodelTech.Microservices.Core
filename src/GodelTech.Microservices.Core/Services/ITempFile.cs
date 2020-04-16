using System;
using System.IO;
using System.Threading.Tasks;

namespace GodelTech.Microservices.Core.Services
{
    public interface ITempFile : IDisposable
    {
        Task WriteAsync(Stream stream);
        string Path { get; }
        FileStream OpenRead();
    }
}