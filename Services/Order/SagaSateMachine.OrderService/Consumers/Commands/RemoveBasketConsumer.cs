using MassTransit;
using SagaSateMachine.Common.Abstractions.Consumers;
using SagaSateMachine.Common.IntegrationEvents;

namespace SagaSateMachine.OrderService.Consumers.Commands
{
    public class RemoveBasketConsumer : ConsumerBase<Command.RemoveBasket>
    {
        private readonly ILogger<RemoveBasketConsumer> _logger;

        public RemoveBasketConsumer(ILogger<RemoveBasketConsumer> logger)
        {
            _logger = logger;
        }

        protected override async Task ConsumeInternal(ConsumeContext<Command.RemoveBasket> context)
        {
            _logger.LogInformation("RemoveBasketConsumer: {message}", context.Message);

            DomainEvent.OrderCompleted orderCompleted = new DomainEvent.OrderCompleted
            {
                Id = Guid.NewGuid(),
                OrderId = context.Message.OrderId,
                BasketId = context.Message.BasketId,
                TimeStamp = DateTime.UtcNow,
                TransactionId = context.Message.TransactionId,
                UserId = context.Message.UserId,
                Message = "Order completed!"
            };

            await context.Publish(orderCompleted);
        }
    }
}
