using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;

namespace GodelTech.Microservices.Core.Tests.Fakes.Mvc.CorrelationId
{
    public class FakeResponseFeature : IHttpResponseFeature
    {
        private Func<object, Task> _callback;
        private object _state;

        public int StatusCode { get; set; }

        public string ReasonPhrase { get; set; }

#pragma warning disable CA2227 // Collection properties should be read only
        // implementation of interface
        public IHeaderDictionary Headers { get; set; } = new HeaderDictionary();
#pragma warning restore CA2227 // Collection properties should be read only

        public Stream Body { get; set; }

        public bool HasStarted { get; private set; }

        public void OnStarting(Func<object, Task> callback, object state)
        {
            _callback = callback;
            _state = state;
        }

        public void OnCompleted(Func<object, Task> callback, object state)
        {

        }

        public Task InvokeCallBack()
        {
            HasStarted = true;

            return _callback(_state);
        }
    }
}
