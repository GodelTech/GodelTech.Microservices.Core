# GodeTech.Microservices.Core

### Microservice Platform 

This package is core component used to define microservice architecture and main interfaces.

## Overview

**GodelTech.Microservices.Swagger** project provides initializer which configures Swagger endpoinds and Swagger UI. Default configuration looks as follows:

* Swagger UI can be found at [http://yourwebsite.com/swagger/index.html](http://yourwebsite.com/swagger/index.html)
* Swagger document can be found at [http://yourwebsite/swagger/v1/swagger.json](http://yourwebsite/swagger/v1/swagger.json)
  
Default behavior can be overriden by changing values of intializer's `Options` property or by deriving your customer initializer from `SwaggerInitializer`.

## Quick Start

Simplest usage of swagger initializer may look as follows:

```c#
    public sealed class Startup : MicroserviceStartup
    {
        public Startup(IConfiguration configuration)
            : base(configuration)
        {
        }

        protected override IEnumerable<IMicroserviceInitializer> CreateInitializers()
        {
            ...
            yield return new SwaggerInitializer(Configuration)
            {
                Options =
                {
                    AuthorizeEndpointUrl = "http://authorize.url",
                    TokenEndpointUrl = "http://token.url",
                    DocumentTitle = "CRM API",
                    SupportedScopes = new[]
                    {
                        new ScopeDetails { Name = "Scope1", Description = "Scope description" }
                    }
                }
            };
            ...
        }
    }
```
This code snippet adds swagger endpoints to your application and exposes document for once version of your API.

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


## Links

Microservice framework has number of satelite projects:
* [GodelTech.Microservices.EntityFrameworkCore](https://github.com/GodelTech/GodelTech.Microservices.EntityFrameworkCore). Project contains implementation of repository pattern and can be used to build application data layer.
* [GodelTech.Microservices.Swagger](https://github.com/GodelTech/GodelTech.Microservices.Swagger). Project contains components used to initialize Swagger UI and Swagger document.
* [GodelTech.Microservices.Http](https://github.com/GodelTech/GodelTech.Microservices.Http). Implemntation of HTTP service client which has convinient methods to collaborate with REST APIs.
* [GodelTech.Microservices.SharedServices](https://github.com/GodelTech/GodelTech.Microservices.SharedServices). Project contains set of unitity components which wrap standard static methods. Available wrappers simplify unit test creation by exposing intefaces which mimic static method signatures.