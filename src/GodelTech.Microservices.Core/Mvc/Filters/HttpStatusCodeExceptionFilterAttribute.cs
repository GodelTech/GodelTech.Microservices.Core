using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace GodelTech.Microservices.Core.Mvc.Filters
{
    /// <summary>
    /// Http status code exception filter attribute.
    /// </summary>
    public sealed class HttpStatusCodeExceptionFilterAttribute : ExceptionFilterAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HttpStatusCodeExceptionFilterAttribute"/> class.
        /// </summary>
        /// <param name="statusCode">Status code.</param>
        /// <param name="exceptionType">Type of exception.</param>
        public HttpStatusCodeExceptionFilterAttribute(
            int statusCode,
            Type exceptionType)
        {
            StatusCode = statusCode;
            ExceptionType = exceptionType;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpStatusCodeExceptionFilterAttribute"/> class.
        /// </summary>
        /// <param name="statusCode">Status code.</param>
        /// <param name="exceptionType">Type of exception.</param>
        public HttpStatusCodeExceptionFilterAttribute(
            HttpStatusCode statusCode,
            Type exceptionType)
            : this((int) statusCode, exceptionType)
        {

        }

        /// <summary>
        /// Status code.
        /// </summary>
        public int StatusCode { get; }

        /// <summary>
        /// Type of exception.
        /// </summary>
        public Type ExceptionType { get; }

        /// <inheritdoc />
        public override void OnException(ExceptionContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));

            var exceptions = new List<object>
            {
                context.Exception
            };

            if (context.Exception is AggregateException aggregateException)
            {
                exceptions.AddRange(
                    aggregateException.InnerExceptions
                );
            }

            if (exceptions.All(x => x.GetType() != ExceptionType))
            {
                return;
            }

            context.HttpContext.Response.StatusCode = StatusCode;
            context.Result = new ObjectResult(
                new ExceptionFilterResultModel
                {
                    ErrorMessage = context.Exception.Message
                }
            );
        }
    }
}
