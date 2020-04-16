using System;
using GodelTech.Microservices.Core.Collaborators.RequestHandlers;

namespace GodelTech.Microservices.Core.Collaborators
{
    public interface IRequestContentHandlerFactory
    {
        IRequestContentHandler Create(Type type);
    }
}