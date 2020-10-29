namespace GodelTech.Microservices.Core.Services
{
    public interface ICorrelationIdAccessor
    {
        string GetCorrelationId();
    }
}
