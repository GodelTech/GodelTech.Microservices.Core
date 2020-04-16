namespace GodelTech.Microservices.Core.Collaborators
{
    public interface IServiceClientFactory
    {
        IServiceClient Create(string serviceName, bool returnDefaultOn404 = false);
    }
}