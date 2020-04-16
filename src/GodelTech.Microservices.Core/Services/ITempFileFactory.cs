namespace GodelTech.Microservices.Core.Services
{
    public interface ITempFileFactory
    {
        ITempFile Create(string tempFolder);
    }
}