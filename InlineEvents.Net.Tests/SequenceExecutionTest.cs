using Moq;
using InlineEvents.Net;

namespace InlineEvents.Net.Tests
{
    public class SequenceExecutionTest
    {
        [Fact]
        public async Task LoginPublisher_WithOrderedHandlers_ExecutesInCorrectSequence()
        {
            // Arrange
            var log = new ExecutionLog();
            var mockResolver = new Mock<IHandlerResolver>(); // Mock YOUR interface!

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
    }
}



