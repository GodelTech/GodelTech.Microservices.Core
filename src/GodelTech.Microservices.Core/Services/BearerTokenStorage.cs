using System;
using System.Threading;

namespace GodelTech.Microservices.Core.Services
{
    public class BearerTokenStorage : IBearerTokenStorage
    {
        private static readonly AsyncLocal<string> AccessTokenStorage = new AsyncLocal<string>();

        public IDisposable SetAccessToken(string value)
        {
            var previousValue = AccessTokenStorage.Value;

            AccessTokenStorage.Value = value;

            return new DisposableAction(() => AccessTokenStorage.Value = previousValue);
        }

        public string GetAccessToken()
        {
            return AccessTokenStorage.Value;
        }
    }
}
