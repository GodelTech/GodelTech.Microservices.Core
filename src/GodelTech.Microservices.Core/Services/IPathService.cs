namespace GodelTech.Microservices.Core.Services
{
    public interface IPathService
    {
        string GetFullPath(string path);
        string Combine(params string[] paths);
        string GetDirectoryName(string path);
        string GetExtension(string path);
        string GetFileName(string path);
    }
}