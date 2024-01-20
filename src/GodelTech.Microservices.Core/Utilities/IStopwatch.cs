namespace GodelTech.Microservices.Core.Utilities
{
    /// <summary>
    /// Timer.
    /// </summary>
    public interface IStopwatch
    {
        /// <summary>
        /// Start.
        /// </summary>
        void Start();

        /// <summary>
        /// Stop.
        /// </summary>
#pragma warning disable CA1716
        void Stop();
#pragma warning restore CA1716

        /// <summary>
        /// Get the elapsed time in milliseconds.
        /// </summary>
        long ElapsedMilliseconds { get; }
    }
}
