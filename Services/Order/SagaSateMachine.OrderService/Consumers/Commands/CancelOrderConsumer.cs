using MassTransit;
using SagaSateMachine.Common.Abstractions.Consumers;
using SagaSateMachine.Common.IntegrationEvents;

namespace SagaSateMachine.OrderService.Consumers.Commands;

public class CancelOrderConsumer : ConsumerBase<Command.CancelOrder>
{
    private readonly ILogger<CancelOrderConsumer> _logger;

    public CancelOrderConsumer(ILogger<CancelOrderConsumer> logger)
    {
        _logger = logger;
    }

    protected override async Task ConsumeInternal(ConsumeContext<Command.CancelOrder> context)
    {
        _logger.LogInformation("CancelOrderConsumer: {message}", context.Message);

        _logger.LogInformation("Cancel Order successfully!");

        DomainEvent.OrderCancelled orderCancelled = new DomainEvent.OrderCancelled
        {
            Id = Guid.NewGuid(),
            TimeStamp = DateTime.Now,
            TransactionId = context.Message.TransactionId,
            UserId = context.Message.UserId,
        };

        await context.Publish(orderCancelled);
    }
}
