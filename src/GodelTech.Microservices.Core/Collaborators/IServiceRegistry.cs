namespace GodelTech.Microservices.Core.Collaborators
{
    public interface IServiceRegistry
    {
        IServiceConfig GetConfig(string serviceName);
    }
}