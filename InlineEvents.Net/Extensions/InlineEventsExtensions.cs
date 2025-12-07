using InlineEvents.Net.Dispatcher;
using Microsoft.Extensions.DependencyInjection;

namespace InlineEvents.Net.Extensions
{
    public static class InlineEventsExtensions
    {
        public static IServiceCollection AddInlineEvents(this IServiceCollection services)
        {
            services.AddTransient<InlineEventDispatcher>();
            return services;
        }
    }
}
