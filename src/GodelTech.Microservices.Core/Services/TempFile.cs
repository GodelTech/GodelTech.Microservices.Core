using System;
using System.IO;
using System.Threading.Tasks;

namespace GodelTech.Microservices.Core.Services
{
    public class TempFile : ITempFile
    {
        private readonly IFileService _fileService;
        public string Path { get; }

        public TempFile(string path, IFileService fileService)
        {
            if (string.IsNullOrWhiteSpace(path))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(path));

            Path = path;
            _fileService = fileService ?? throw new ArgumentNullException(nameof(fileService));
        }

        public FileStream OpenRead()
        {
            return File.OpenRead(Path);
        }

        public async Task WriteAsync(Stream stream)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));

            using (var fileStream = new FileStream(Path, FileMode.Create, FileAccess.ReadWrite, FileShare.None))
            {
                await stream.CopyToAsync(fileStream);
            }
        }

        public void Dispose()
        {
            if (_fileService.Exists(Path))
                _fileService.Delete(Path);
        }
    }
}