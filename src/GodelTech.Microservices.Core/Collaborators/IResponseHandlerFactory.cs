using System;
using GodelTech.Microservices.Core.Collaborators.ResponseHandlers;

namespace GodelTech.Microservices.Core.Collaborators
{
    public interface IResponseHandlerFactory
    {
        IResponseHandler Create(Type type);
    }
}