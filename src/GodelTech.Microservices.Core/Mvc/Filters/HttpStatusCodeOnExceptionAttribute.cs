using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace GodelTech.Microservices.Core.Mvc.Filters
{
    public class HttpStatusCodeOnExceptionAttribute : ExceptionFilterAttribute
    {
        private readonly int _statusCode;
        private readonly Type[] _exceptionTypes;

        public HttpStatusCodeOnExceptionAttribute(int statusCode, params Type[] exceptionTypes)
        {
            if (exceptionTypes == null)
                throw new ArgumentNullException(nameof(exceptionTypes));

            _statusCode = statusCode;
            _exceptionTypes = exceptionTypes;
        }

        public override void OnException(ExceptionContext context)
        {
            var exception = context.Exception;

            if (exception is AggregateException)
                exception = exception.InnerException;

            if (!_exceptionTypes.Any(x => x.IsInstanceOfType(exception)))
            {
                base.OnException(context);
                return;
            }

            context.HttpContext.Response.StatusCode = _statusCode;
            context.Result = new ObjectResult(GetErrorContent(context.Exception));    
        }

        protected virtual object GetErrorContent(Exception exception)
        {
            return new
            {
                errorMessage = exception.Message
            };
        }
    }
}
