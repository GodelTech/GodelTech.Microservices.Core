using System;

namespace GodelTech.Microservices.Core.Timer
{
    /// <summary>
    /// Timer.
    /// </summary>
    public interface IStopwatch
    {
        /// <summary>
        /// Start new.
        /// </summary>
        IStopwatch StartNew();

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
