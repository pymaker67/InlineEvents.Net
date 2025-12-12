using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace InlineEvents.Net
{
    // The default implementation that wires the contract to IServiceProvider
    public class ServiceProviderHandlerResolver(IServiceProvider serviceProvider) : IHandlerResolver
    {
        public IEnumerable<IEventHandlerInline<TEvent>> GetHandlers<TEvent>() where TEvent : class
        {
            // This is where you use the standard DI extension method
            return serviceProvider.GetServices<IEventHandlerInline<TEvent>>();
        }
    }
}
