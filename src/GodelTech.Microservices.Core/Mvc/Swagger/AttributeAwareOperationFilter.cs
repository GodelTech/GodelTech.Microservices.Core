using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Controllers;

namespace GodelTech.Microservices.Core.Mvc.Swagger
{
    public abstract class AttributeAwareOperationFilter
    {
        protected static IEnumerable<T> GetMethodOrControllerAttributes<T>(ApiDescription apiDescription) where T : Attribute
        {
            return GetMethodAttributes<T>(apiDescription).Concat(GetControllerAttributes<T>(apiDescription));
        }

        protected static IEnumerable<T> GetControllerAttributes<T>(ApiDescription apiDescription) where T : Attribute
        {
            var controller = GetControllerType(apiDescription);
            return CustomAttributeExtensions.GetCustomAttributes(controller.GetTypeInfo(), typeof(T), false).Cast<T>();
        }

        protected static Type GetControllerType(ApiDescription apiDescription)
        {
            return AsControllerDescriptor(apiDescription.ActionDescriptor).ControllerTypeInfo.AsType();
        }

        protected static IEnumerable<T> GetMethodAttributes<T>(ApiDescription apiDescription) where T : Attribute
        {
            var method = AsControllerDescriptor(apiDescription.ActionDescriptor).MethodInfo;
            return CustomAttributeExtensions.GetCustomAttributes(method, typeof(T), false).Cast<T>();
        }

        protected static ControllerActionDescriptor AsControllerDescriptor(ActionDescriptor actionDescriptor)
        {
            return actionDescriptor as ControllerActionDescriptor;
        }
    }
}