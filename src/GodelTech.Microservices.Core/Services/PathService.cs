using System.IO;

namespace GodelTech.Microservices.Core.Services
{
    public class PathService : IPathService
    {
        public string GetFullPath(string path)
        {
            return Path.GetFullPath(path);
        }

        public string Combine(params string[] paths)
        {
            return Path.Combine(paths);
        }

        public string GetDirectoryName(string path)
        {
            return Path.GetDirectoryName(path);
        }

        public string GetExtension(string path)
        {
            return Path.GetExtension(path);
        }

        public string GetFileName(string path)
        {
            return Path.GetFileName(path);
        }
    }
}