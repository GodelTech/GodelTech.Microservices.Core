using System;
using System.Net;

namespace GodelTech.Microservices.Core.Mvc.Filters
{
    public class BadRequestOnExceptionAttribute : HttpStatusCodeOnExceptionAttribute
    {
        public BadRequestOnExceptionAttribute(params Type[] exceptionTypes) 
            : base((int)HttpStatusCode.BadRequest, exceptionTypes)
        {
        }
    }
}