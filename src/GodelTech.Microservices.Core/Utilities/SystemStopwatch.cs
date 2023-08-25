using System.Diagnostics;

namespace GodelTech.Microservices.Core.Utilities
{
    internal class SystemStopwatch : IStopwatch
    {
        protected Stopwatch Stopwatch { get; } = new Stopwatch();

        public void Start() => Stopwatch.Start();
        public void Stop() => Stopwatch.Stop();
        public long ElapsedMilliseconds => Stopwatch.ElapsedMilliseconds;
    }
}
