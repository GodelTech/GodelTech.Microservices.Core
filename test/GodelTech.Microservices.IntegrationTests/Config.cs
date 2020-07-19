using Xunit;

[assembly: CollectionBehavior(DisableTestParallelization = true)]

// Tests must be executed sequentially in order to avoid concurrency issues when InitializerFactory is set by different threads.
// Detailed information about ASP.NET Core integration tests can be found here:
// https://docs.microsoft.com/en-us/aspnet/core/test/integration-tests?view=aspnetcore-3.1
