namespace GodelTech.Microservices.Core.Services
{
    public interface IJsonSerializer
    {
        T Deserialize<T>(string content);
        string Serialize(object data);
    }
}