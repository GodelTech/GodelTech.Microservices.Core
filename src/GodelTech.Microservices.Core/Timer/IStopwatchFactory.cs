namespace GodelTech.Microservices.Core.Timer
{
    /// <summary>
    /// A factory for creating <see cref="IStopwatch" /> instances.
    /// </summary>
    public interface IStopwatchFactory
    {
        /// <summary>
        /// Creates an <see cref="IStopwatch"/> instance.
        /// </summary>
        /// <returns>The <see cref="IStopwatch"/> instance.</returns>
        IStopwatch Create();
    }
}
