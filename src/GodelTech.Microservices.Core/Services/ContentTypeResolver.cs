using System;

namespace GodelTech.Microservices.Core.Services
{
    public partial class ContentTypeResolver : IContentTypeResolver
    {
        private readonly IPathService _pathService;

        public ContentTypeResolver(IPathService pathService)
        {
            _pathService = pathService ?? throw new ArgumentNullException(nameof(pathService));
        }

        public string GetByFilePath(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(filePath));

            var extensions = _pathService.GetExtension(filePath);

            if (extensions.StartsWith("."))
                extensions = extensions.Substring(1);

            return MimeMap.ContainsKey(extensions) ?
                MimeMap[extensions] :
                "application/octet-stream";
        }
    }
}