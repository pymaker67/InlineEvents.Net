using System;
using System.Collections.Generic;
using System.Text;

namespace InlineEvents.Net
{
    public interface IEventHandlerInline<TEvent>
    {
        Task HandleInline(TEvent @event);
    }
}
