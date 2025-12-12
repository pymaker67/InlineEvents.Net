using Microsoft.Extensions.DependencyInjection;

namespace InlineEvents.Net
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddInlineEvents(this IServiceCollection services)
        {
            // 1. AUTOMATIC HANDLER DISCOVERY (Your Code Block)
            services.Scan(scan => scan
                .FromApplicationDependencies() // Look across all loaded application assemblies
                .AddClasses(classes => classes.AssignableTo(typeof(IEventHandlerInline<>)))
                .AsImplementedInterfaces()    // Register as the IEventHandlerInline<TEvent> interface
                .WithTransientLifetime());    // Ensure a new instance is created for each dispatch

            // 2. REGISTER CORE INFRASTRUCTURE (The Glue)

            // Register the concrete resolver (which uses IServiceProvider to get handlers)
            services.AddSingleton<IHandlerResolver, ServiceProviderHandlerResolver>();

            // Register the dispatcher (which uses IHandlerResolver to execute logic)
            services.AddSingleton<InlineEventDispatcher>();

            return services;
        }
    }
}
