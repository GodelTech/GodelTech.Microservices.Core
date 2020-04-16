using System;
using System.Threading.Tasks;
using GodelTech.Microservices.Core.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;

namespace GodelTech.Microservices.Core.Mvc.Middlewares
{
    public class AccessTokenMiddleware
    {
        private readonly IBearerTokenStorage _bearerTokenStorage;
        private readonly RequestDelegate _next;

        public AccessTokenMiddleware(
            IBearerTokenStorage bearerTokenStorage,
            RequestDelegate next)
        {
            _bearerTokenStorage = bearerTokenStorage ?? throw new ArgumentNullException(nameof(bearerTokenStorage));
            _next = next ?? throw new ArgumentNullException(nameof(next));
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var accessToken = await context.GetTokenAsync("access_token");

            using (_bearerTokenStorage.SetAccessToken(accessToken))
            {
                await _next.Invoke(context);
            }
        }
    }
}