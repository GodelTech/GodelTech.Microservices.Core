using System;
using System.Net;

namespace GodelTech.Microservices.Core.Mvc.Filters
{
    public class NotFoundOnExceptionAttribute : HttpStatusCodeOnExceptionAttribute
    {
        public NotFoundOnExceptionAttribute(params Type[] exceptionTypes)
            : base((int)HttpStatusCode.NotFound, exceptionTypes)
        {
        }
    }
}