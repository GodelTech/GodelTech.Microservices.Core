using System.Diagnostics;

namespace GodelTech.Microservices.Core.Timer
{
    internal class SystemStopwatch : IStopwatch
    {
        protected Stopwatch Stopwatch { get; } = new Stopwatch();

        public IStopwatch StartNew()
        {
            var stopwatch = new SystemStopwatch();
            stopwatch.Start();
            return stopwatch;
        }

        public void Start() => Stopwatch.Start();
        public void Stop() => Stopwatch.Stop();
        public long ElapsedMilliseconds => Stopwatch.ElapsedMilliseconds;
    }
}
