# InlineEvents.Net

InlineEvents.Net is a sequential event-routing framework for .NET, built for tasks that require an immediate response 
following an event. It is the ideal choice for scenarios where background processing is too risky or complex, ensuring 
that all application rules are satisfied before the calling thread returns.

## Core Philosophy
Unlike traditional message buses that favor eventual consistency, InlineEvents.Net focuses on Immediate Consistency. 
It ensures that your side effects (Application Rules) are executed in the same execution context and database transaction 
as your main logic.

### Installation

To install InlineEvents.Net, run the following command in your project directory:

```bash
dotnet add package InlineEvents.Net
```

### Usage

In your `Program.cs`, register InlineEvents.Net with the service collection:

```csharp
// Program.cs

// 1. Register the Core InlineEvents.Net Library
// This finds all IEventHandlerInline<T> classes and registers the Dispatcher (Singleton)
builder.Services.AddInlineEvents(); 

// 2. Register the Specific Publisher Abstraction (Optional)
// This is the gateway the application services will use. We use Scoped for typical web requests.
builder.Services.AddScoped<ILoginEventPublisher, LoginEventPublisher>();

// 3. Create event type
// The Event Type
public record UserLoggedIn(string UserId);

// 4. Create publisher interface with implementation
// The Publisher Interface (Specific to the Login Event)

public interface ILoginEventPublisher
{
    Task PublishAsync(UserLoggedIn @event);
}

// The Publisher Implementation (Consumes the dispatcher)
public class LoginEventPublisher(IInlineEventDispatcher dispatcher) : ILoginEventPublisher
{
    public Task PublishAsync(UserLoggedIn @event) => dispatcher.Dispatch(@event);
}

### Handler Ordering (Optional)

By default, handlers just implement `IEventHandlerInline<TEvent>` 

// Classes for the Unit Test Scope (can be defined inside your test class)

public record UserLoggedIn(string UserId);

public class ExecutionLog
{
	public List<string> Sequence { get; } = [];
}

// Handler implementations used for mocking:
public class MockHandlerA(ExecutionLog log) : IEventHandlerInline<UserLoggedIn>, ISequenceHandlerInline
{
	public int Order => 100; // Low Priority
	public Task HandleInline(UserLoggedIn @event) 
	{ 
		log.Sequence.Add("Handler A (Order 100)"); 
		return Task.CompletedTask; 
	}
}

public class MockHandlerB(ExecutionLog log) : IEventHandlerInline<UserLoggedIn>, ISequenceHandlerInline
{
	public int Order => -100; // High Priority
	public Task HandleInline(UserLoggedIn @event) 
	{ 
		log.Sequence.Add("Handler B (Order -100)"); 
		return Task.CompletedTask; 
	}
}
```

### Test

```csharp
[Fact]
public async Task LoginPublisher_WithOrderedHandlers_ExecutesInCorrectSequence()
{
    // Arrange
    var log = new ExecutionLog();
    var mockResolver = new Mock<IHandlerResolver>();

    // 1. Create Handlers. Deliberately unsorted in the array.
    var unsortedHandlers = new IEventHandlerInline<UserLoggedIn>[]
    {
        new MockHandlerA(log),
        new MockHandlerB(log)
    };

    // 2. Setup the mock Resolver to return the unsorted list 
    // when the dispatcher asks for handlers of type UserLoggedIn.
    mockResolver
        .Setup(r => r.GetHandlers<UserLoggedIn>())
        .Returns(unsortedHandlers);

    // 3. Instantiate the Dispatcher, injecting the mock resolver.
    var dispatcher = new InlineEventDispatcher(mockResolver.Object);

    // Act
    await dispatcher.Dispatch(new UserLoggedIn("test-user"));

    // Assert
    // Verify that Handler B (Order -100) ran before Handler A (Order 100).
    Assert.Equal(2, log.Sequence.Count);
    Assert.Equal("Handler B (Order -100)", log.Sequence[0]);
    Assert.Equal("Handler A (Order 100)", log.Sequence[1]);
}

```