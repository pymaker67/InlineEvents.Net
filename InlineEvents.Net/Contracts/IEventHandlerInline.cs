
namespace InlineEvents.Net.Contracts
{
    public interface IEventHandlerInline<TEvent>
    {
        Task HandleInline(TEvent @event);
    }
}
