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
        void Stop();

        /// <summary>
        /// Get the elapsed time in milliseconds.
        /// </summary>
        long ElapsedMilliseconds { get; }
    }
}
