using System;
using System.Net.Http;

namespace GodelTech.Microservices.IntegrationTests.Utils
{
    public static class HttpResponseMessageExtensions
    {
        public static string GetText(this HttpResponseMessage response)
        {
            if (response == null) 
                throw new ArgumentNullException(nameof(response));

            return response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
        }
    }
}
