namespace InlineEvents.Net
{
    public interface IHandlerResolver
    {
        // The method the Dispatcher will call to get handlers
        IEnumerable<IEventHandlerInline<TEvent>> GetHandlers<TEvent>() where TEvent : class;
    }
}
