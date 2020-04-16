using System;

namespace GodelTech.Microservices.Core.Services
{
    public class TempFileFactory : ITempFileFactory
    {
        private readonly IFileService _fileService;
        private readonly IPathService _pathService;
        private readonly IGuidFactory _guidFactory;

        public TempFileFactory(
            IFileService fileService,
            IPathService pathService,
            IGuidFactory guidFactory)
        {
            _fileService = fileService ?? throw new ArgumentNullException(nameof(fileService));
            _pathService = pathService ?? throw new ArgumentNullException(nameof(pathService));
            _guidFactory = guidFactory ?? throw new ArgumentNullException(nameof(guidFactory));
        }

        public ITempFile Create(string tempFolder)
        {
            if (string.IsNullOrWhiteSpace(tempFolder))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(tempFolder));

            return new TempFile(
                _pathService.Combine(tempFolder, _guidFactory.NewAsString()),
                _fileService);
        }
    }
}
