using System;
using System.Text;

namespace GodelTech.Microservices.Core.Services
{
    public abstract class UrlBuilderBase
    {
        private readonly IHostConfig _hostConfig;

        protected UrlBuilderBase(IHostConfig hostConfig)
        {
            _hostConfig = hostConfig ?? throw new ArgumentNullException(nameof(hostConfig));
        }

        protected string ToAbsoluteUrl(string relativeUrl)
        {
            var builder = new StringBuilder();

            var baseAddress = _hostConfig.BaseAddress;

            if (!baseAddress.EndsWith("/"))
                baseAddress += "/";

            return builder
                .Append(baseAddress)
                .Append(relativeUrl)
                .ToString();
        }
    }
}