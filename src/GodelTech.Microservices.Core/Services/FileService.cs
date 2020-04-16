using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace GodelTech.Microservices.Core.Services
{
    public class FileService : IFileService
    {
        public async Task<string> ReadAllTextAsync(string filePath)
        {
            return await File.ReadAllTextAsync(filePath);
        }

        public FileStream OpenRead(string filePath)
        {
            return File.OpenRead(filePath);
        }

        public FileStream OpenWrite(string filePath)
        {
            return File.OpenWrite(filePath);
        }

        public async Task WriteAllTextAsync(string filePath, string content)
        {
            await File.WriteAllTextAsync(filePath, content);
        }

        public bool Exists(string path)
        {
            return File.Exists(path);
        }

        public void Delete(string path)
        {
            File.Delete(path);
        }

        public IEnumerable<string> FindAll(string path, string mask)
        {
            if (string.IsNullOrWhiteSpace(path))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(path));
            if (string.IsNullOrWhiteSpace(mask))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(mask));

            foreach (var childDirectory in Directory.GetDirectories(path))
            {
                foreach (var file in FindAll(childDirectory, mask))
                    yield return file;
            }

            foreach (var file in Directory.GetFiles(path, mask))
            {
                yield return file;
            }
        }

        public Task<byte[]> ReadAllBytesAsync(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(filePath));

            return File.ReadAllBytesAsync(filePath);
        }
    }
}
