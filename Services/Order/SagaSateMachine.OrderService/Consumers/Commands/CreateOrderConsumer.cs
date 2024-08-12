using MassTransit;
using SagaSateMachine.Common.Abstractions.Consumers;
using SagaSateMachine.Common.IntegrationEvents;

namespace SagaSateMachine.OrderService.Consumers.Commands
{
    public class CreateOrderConsumer : ConsumerBase<Command.CreateOrder>
    {
        private readonly ILogger<CreateOrderConsumer> _logger;

        public CreateOrderConsumer(ILogger<CreateOrderConsumer> logger)
        {
            _logger = logger;
        }

        protected override async Task ConsumeInternal(ConsumeContext<Command.CreateOrder> context)
        {
            _logger.LogInformation("CreateOrderConsumer: {message}", context.Message);

            //var rdValue = (new Random().Next(10) + 1);
            var rdValue = 2;

            try
            {
                if (rdValue < 5)
                {
                    // Create Order
                    _logger.LogInformation("Create Order Completed");

                    DomainEvent.OrderCreated orderCreated = new DomainEvent.OrderCreated
                    {
                        Id = context.Message.Id,
                        OrderId = context.Message.OrderId,
                        UserId = context.Message.UserId,
                        TimeStamp = DateTime.Now,
                        TransactionId = context.Message.TransactionId,
                    };

                    await context.Publish(orderCreated);
                }
                else
                {
                    throw new InvalidOperationException("Order incompleted");
                }
            }
            catch (Exception e)
            {
                DomainEvent.OrderCreatedFailed orderCreatedFailed = new DomainEvent.OrderCreatedFailed
                {
                    Id = Guid.NewGuid(),
                    Reason = $"{e.Message}",
                    TimeStamp = DateTime.Now,
                    TransactionId= context.Message.TransactionId,   
                    UserId = context.Message.UserId,
                };

                await context.Publish(orderCreatedFailed);
            }
        }
    }
}
