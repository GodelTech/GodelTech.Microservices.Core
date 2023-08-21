namespace GodelTech.Microservices.Core.Timer
{
    internal class SystemStopwatchFactory : IStopwatchFactory
    {
        public IStopwatch Create()
        {
            return new SystemStopwatch();
        }
    }
}
