using System.Threading;

namespace GodelTech.Microservices.Core.Mvc.CorrelationId
{
    /// <summary>
    /// Provides an implementation of <see cref="ICorrelationIdContextAccessor" />.
    /// </summary>
    public class CorrelationIdContextAccessor : ICorrelationIdContextAccessor
    {
        private static readonly AsyncLocal<CorrelationIdContextHolder> CorrelationIdContextCurrent = new AsyncLocal<CorrelationIdContextHolder>();

        /// <inheritdoc/>
        public CorrelationIdContext? CorrelationIdContext
        {
            get
            {
                return CorrelationIdContextCurrent.Value?.Context;
            }
            set
            {
                var holder = CorrelationIdContextCurrent.Value;
                if (holder != null)
                {
                    // Clear current CorrelationIdContext trapped in the AsyncLocals, as its done.
                    holder.Context = null;
                }

                if (value != null)
                {
                    // Use an object indirection to hold the CorrelationIdContext in the AsyncLocal,
                    // so it can be cleared in all ExecutionContexts when its cleared.
                    CorrelationIdContextCurrent.Value = new CorrelationIdContextHolder { Context = value };
                }
            }
        }

        private class CorrelationIdContextHolder
        {
            public CorrelationIdContext? Context;
        }
    }
}