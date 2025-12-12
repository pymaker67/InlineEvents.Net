namespace InlineEvents.Net
{
    public interface IInlineEventDispatcher
    {
        Task Dispatch<TEvent>(TEvent eventMessage) where TEvent : class;
    }
}
