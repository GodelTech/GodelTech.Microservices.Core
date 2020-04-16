using System;

namespace GodelTech.Microservices.Core.Services
{
    /// <inheritdoc />
    /// <summary>
    /// Better sytnax for context operation.
    /// Wraps a delegate that is executed when the Dispose method is called.
    /// This allows to do context sensitive things easily.
    /// Basically, it mimics Java's anonymous classes.
    /// </summary>
    public class DisposableAction : IDisposable
    {
        private readonly Action _action;

        /// <summary>
        /// Initializes a new instance of the <see cref="DisposableAction"/> class.
        /// </summary>
        /// <param name="action">The action to execute on dispose</param>
        public DisposableAction(Action action)
        {
            _action = action ?? throw new ArgumentNullException(nameof(action));
        }

        /// <inheritdoc />
        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _action();
            }
        }
    }
}