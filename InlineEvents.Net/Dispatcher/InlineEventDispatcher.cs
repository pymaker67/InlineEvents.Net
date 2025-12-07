

using InlineEvents.Net.Contracts;
using Microsoft.Extensions.DependencyInjection;

namespace InlineEvents.Net.Dispatcher
{
    public class InlineEventDispatcher(IServiceProvider serviceProvider)
    {
        private readonly IServiceProvider _serviceProvider = serviceProvider;

        public async Task Dispatch<TEvent>(TEvent @event)
        {
            // Resolve all handlers that match TEvent
            var handlers = _serviceProvider.GetServices<IEventHandlerInline<TEvent>>();
            foreach (var handler in handlers)
            {
                await handler.HandleInline(@event);
            }
        }
    }
}
