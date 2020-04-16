using System;

namespace GodelTech.Microservices.Core.Services
{
    public interface IBearerTokenStorage
    {
        IDisposable SetAccessToken(string value);
        string GetAccessToken();
    }
}