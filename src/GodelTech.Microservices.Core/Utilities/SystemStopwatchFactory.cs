namespace GodelTech.Microservices.Core.Utilities
{
    internal class SystemStopwatchFactory : IStopwatchFactory
    {
        public IStopwatch Create()
        {
            return new SystemStopwatch();
        }
    }
}
