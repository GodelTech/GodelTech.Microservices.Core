using System;

namespace GodelTech.Microservices.Core.Utilities
{
    internal class SystemGuid : IGuid
    {
        public Guid NewGuid() => Guid.NewGuid();
    }
}
