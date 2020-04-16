using System;
using System.IO.Compression;

namespace GodelTech.Microservices.Core.Services
{
    public class ZipService : IZipService
    {
        public void ExtractToDirectory(string archivePath, string outputFolderPath)
        {
            if (string.IsNullOrWhiteSpace(archivePath))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(archivePath));
            if (string.IsNullOrWhiteSpace(outputFolderPath))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(outputFolderPath));

            ZipFile.ExtractToDirectory(archivePath, outputFolderPath);
        }
    }
}