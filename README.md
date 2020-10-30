# GodeTech.Microservices.Core

## Overview
`Godel.Microservice.Core` project contains base components and interfaces of Microservices Framework. Whole idea of framework is based on components called **initializers**. 

Initializer contract is defined as follows:

```c#
    public interface IMicroserviceInitializer
    {
        void ConfigureServices(IServiceCollection services);
        void Configure(IApplicationBuilder app, IWebHostEnvironment env);
    }
```

It's easy to see that contracts mimics signature of typical `Startup` class. Main purpose of initializer is encapsulation of ASP.NET Core application pipeline configuration. E.g. each aspect of pipeline (REST API, HTTPS, Static Files and etc) is configured by dedicated initializer. As result complex unmaintainable `Startup` class is splitted into number of initializers which are sequentially executed by subclass of `MicroserviceStartup`. In order to save few lines of code during initializer development `MicroserviceInitializerBase` class can be used.

`MicroserviceStartup` is recommended base class for `Startup` class. In comparison to ordinary `Startup` this class just few differences:

1. It accepts `IConfiguration` as constructor parameter and stores it in property named `Configuration`.
2. Abstract method `CreateInitializers` must be created by child classes. This method is responsible for initializer chain creation.
3. Virtual methods `ConfigureServices()` and `Configure()` are defined.
4. `ConfigureServices()` and `Configure()` invoke corresponding methods of initializers to configure ASP.NET Core application.
5. Few common services are registered by `ConfigureServices()`

Other than this no other logic is included into `MicroserviceStartup` class.

## Quick Start

In order to use microservice framework few simple steps are required:

1. Create ASP.NET Website application using **Visual Studio** or **dotnet cli**.
2. Reference latest version of `Godel.Microservice.Core` nuget package and optionally satellite packages you would like to use.
3. Update your `Startup.cs` file to create initializers used to configure pipeline.

### REST API configuration

Please use the following snippet to configure service which uses REST API only:

```c#
    public class Startup : MicroserviceStartup
    {
        public Startup(IConfiguration configuration)
            : base(configuration)
        {
			
        }

        protected override IEnumerable<IMicroserviceInitializer> CreateInitializers()
        {
            yield return new DeveloperExceptionPageInitializer(Configuration);
            yield return new HttpsInitializer(Configuration);

            yield return new GenericInitializer(null, (app, env) => app.UseRouting());
            yield return new GenericInitializer(null, (app, env) => app.UseAuthentication());

            yield return new ApiInitializer(Configuration);
        }
    }
```

### Razor Pages Configuration

Please use the following snippet to configure service which use Razon Pages only:

```c#
    public class Startup : MicroserviceStartup
    {
        public Startup(IConfiguration configuration)
            : base(configuration)
        {
			
        }

        protected override IEnumerable<IMicroserviceInitializer> CreateInitializers()
        {
            yield return new DeveloperExceptionPageInitializer(Configuration)
            {
                ErrorHandlingPath = "/Error"
            };

            yield return new HttpsInitializer(Configuration);

            yield return new GenericInitializer(null, (app, env) => app.UseStaticFiles());
            yield return new GenericInitializer(null, (app, env) => app.UseRouting());
            yield return new GenericInitializer(null, (app, env) => app.UseAuthentication());

            yield return new RazorPagesInitializer(Configuration);
        }
    }
```

### ASP.NET MVC Configuration

Please use the following snippet to configure service which use ASP.NET MVC only (REST API will be impicitly registered too):

```c#
    public class Startup : MicroserviceStartup
    {
        public Startup(IConfiguration configuration)
            : base(configuration)
        {
			
        }

        protected override IEnumerable<IMicroserviceInitializer> CreateInitializers()
        {
            yield return new DeveloperExceptionPageInitializer(Configuration)
            {
                ErrorHandlingPath = "/Error"
            };

            yield return new HttpsInitializer(Configuration);

            yield return new GenericInitializer(null, (app, env) => app.UseStaticFiles());
            yield return new GenericInitializer(null, (app, env) => app.UseRouting());
            yield return new GenericInitializer(null, (app, env) => app.UseAuthentication());

            yield return new MvcInitializer(Configuration);
        }
    }
```

### Mixed usage

REST API, Razor Pages and ASP.NET MVC initializers can be used inside one application. Please avoid situations when the same url can be handled by different components. If this happens first component specified in initializer list wins.

The following snippet is example of microservice using Razor Pages and REST APIs:

```c#
    public class Startup : MicroserviceStartup
    {
        public Startup(IConfiguration configuration)
            : base(configuration)
        {
			
        }

        protected override IEnumerable<IMicroserviceInitializer> CreateInitializers()
        {
            yield return new DeveloperExceptionPageInitializer(Configuration)
            {
                ErrorHandlingPath = "/Error"
            };
                        
            yield return new HttpsInitializer(Configuration);

            yield return new GenericInitializer(null, (app, env) => app.UseStaticFiles());
            yield return new GenericInitializer(null, (app, env) => app.UseRouting());
            yield return new GenericInitializer(null, (app, env) => app.UseAuthentication());

            yield return new ApiInitializer(Configuration);
            yield return new RazorPagesInitializer(Configuration);
        }
    }
```

## Initializers

Framework comes with a number of initializers. Full list of initializers and their description can be found in the following table:

| Initializer | Description |
|---|---|
|`ApiInitializer`| Initializer configures REST API controllers and configures set of frequently used action filters and JSON formatting settings. |
|`RazorPagesInitializer`| Component is reponsible for Razor Pages configuration. |
|`MvcInitializer`| Initializer is reponsible for MVC controllers with views configuration. When this controller is used REST API initialization is also performed. Additional use of `ApiInitializer` is not required. |
|`GenericInitializer`| This initialize accepts lamda function as parameters. These functions are used by `ConfigureServices()` and `Configure()` method. This initializer can be used when single line pipeline configuration is performed and creation of dedicated initializer is not reasonable.|
|`DeveloperExceptionPageInitializer`| Component is reponsible for exception handling pages configuration. If application is executed in `Development` environment developer exception page is used. If environment differs from `Development` exception handler is used. |
|`HttpsInitializer`| HTTP configuration is performed by this initializer. |
|`HealthCheckInitializer`| Health check infrastructure is configured by this initializer.|
|`CommonMiddlewareInitializer`| This class configures number of frequently used middlewares such as `CorrelationIdMiddleware`, `LogUncaughtErrorsMiddleware` and `RequestResponseLoggingMiddleware`. |

## Advanced usage

### Third-party DI container and logging mechanism

ASP.NET Core provides good configuration by default. At the same time certain functionality can't be provided by default services (dynamic proxies, interceptors and etc.). In order to starting using third-party DI framework your `Program.cs` must be updated as follows:

```c#
    public sealed class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .UseServiceProviderFactory(new AutofacServiceProviderFactory())
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
        }
    }
```
This code snippet configures `Autofac` container as service provider used by ASP.NET Core application. In order to register Autofac modules in container the following code must be added to your `Startup` class:

```c#
    public void ConfigureContainer(ContainerBuilder builder)
    {
        builder.RegisterModule(new MyApplicationModule());
    }
```
You can find detailed information regarding configuration in [Autofac documentation](https://autofaccn.readthedocs.io/en/latest/integration/aspnetcore.html#startup-class).

### Third-party logging mechanism

In order to use third-party logging libraries your `Program.cs` file must be updated as follows:

```c#
   public sealed class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .UseSerilog((context, loggerConfiguration) =>
                {
                    loggerConfiguration.ReadFrom.Configuration(context.Configuration);
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
        }
    }
```
This snippet configures `Serilog` as logging provider for ASP.NET Core application.

## Links

Microservice framework has number of satellite projects:
* [GodelTech.Microservices.EntityFrameworkCore](https://github.com/GodelTech/GodelTech.Microservices.EntityFrameworkCore). Project contains implementation of repository pattern and can be used to build application data layer.
* [GodelTech.Microservices.Swagger](https://github.com/GodelTech/GodelTech.Microservices.Swagger). Project contains components used to initialize Swagger UI and Swagger document.
* [GodelTech.Microservices.Security](https://github.com/GodelTech/GodelTech.Microservices.Security). Package contains security initializer for typical ASP.NET Core applications.
* [GodelTech.Microservices.Http](https://github.com/GodelTech/GodelTech.Microservices.Http). Implemntation of HTTP service client which has convinient methods to collaborate with REST APIs.
* [GodelTech.Microservices.SharedServices](https://github.com/GodelTech/GodelTech.Microservices.SharedServices). Project contains set of unitity components which wrap standard static methods. Available wrappers simplify unit test creation by exposing intefaces which mimic static method signatures.