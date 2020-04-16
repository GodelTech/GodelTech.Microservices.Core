namespace GodelTech.Microservices.Core.Services
{
    public interface IZipService
    {
        void ExtractToDirectory(string archivePath, string outputFolderPath);
    }
}