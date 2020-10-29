using System;
using System.Threading;

namespace GodelTech.Microservices.Core.Services
{
    /// <summary>
    /// This class is supposed to be used by Intelliflo.Platform only
    /// </summary>
    public sealed class CorrelationIdAccessor : ICorrelationIdAccessor, ICorrelationIdSetter
    {
        private static readonly AsyncLocal<string> CorrelationIdStorage = new AsyncLocal<string>();

        internal static string CurrentCorrelationId => CorrelationIdStorage.Value;

        public IDisposable SetCorrelationId(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return new DisposableAction(() => { });

            CorrelationIdStorage.Value = id;

            return new DisposableAction(() => CorrelationIdStorage.Value = null);
        }

        public string GetCorrelationId()
        {
            return CurrentCorrelationId;
        }
    }
}
