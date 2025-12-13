namespace InlineEvents.Net
{
    // Top-level dispatcher
    // The core engine implementation
    public class InlineEventDispatcher(IHandlerResolver resolver) : IInlineEventDispatcher
    {
        public async Task Dispatch<TEvent>(TEvent @event) where TEvent : class
        {
            // 1. Resolve handlers using your abstraction
            var handlers = resolver.GetHandlers<TEvent>();

            // 2. Perform the ordering logic (This core logic is now unit-testable!)
            var orderedHandlers = handlers
                .OrderBy(h => (h as ISequenceHandlerInline)?.Order ?? int.MaxValue);

            // 3. Execution...
            foreach (var handler in orderedHandlers)
            {
                await handler.HandleInline(@event);
            }
        }
    }
}
