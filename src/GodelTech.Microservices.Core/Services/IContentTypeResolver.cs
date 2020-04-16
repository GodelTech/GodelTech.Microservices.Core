namespace GodelTech.Microservices.Core.Services
{
    public interface IContentTypeResolver
    {
        string GetByFilePath(string filePath);
    }
}
