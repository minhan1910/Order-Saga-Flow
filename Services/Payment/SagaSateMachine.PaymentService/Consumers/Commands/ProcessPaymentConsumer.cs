using MassTransit;
using SagaSateMachine.Common.Abstractions.Consumers;
using SagaSateMachine.Common.IntegrationEvents;

namespace SagaSateMachine.PaymentService.Consumers.Commands
{
    public class ProcessPaymentConsumer : ConsumerBase<Command.ProcessPayment>
    {
        private readonly ILogger<ProcessPaymentConsumer> _logger;

        public ProcessPaymentConsumer(ILogger<ProcessPaymentConsumer> logger)
        {
            _logger = logger;
        }

        protected override async Task ConsumeInternal(ConsumeContext<Command.ProcessPayment> context)
        {
            _logger.LogInformation("ProcessPaymentConsumer: {message}", context.Message);

            using var source = new CancellationTokenSource(TimeSpan.FromSeconds(30));

            Random rd = new Random();
            //var rdValue = rd.Next(10) + 1;
            int rdValue = 2;

            try
            {
                if (rdValue > 5)
                {
                    DomainEvent.PaymentProcessed paymentProcessed = new DomainEvent.PaymentProcessed
                    {
                        Id = Guid.NewGuid(),
                        OrderId = context.Message.OrderId,
                        PaymentId = Guid.NewGuid(),
                        TimeStamp = DateTime.Now,
                        TransactionId = context.Message.TransactionId
                    };

                    await context.Publish(paymentProcessed, source.Token);
                }
                else
                {
                    // Failed
                    throw new Exception("Timeout payment");
                }
            }
            catch (Exception e)
            {
                // Failed
                DomainEvent.PaymentProcessedFailed paymentFailed = new DomainEvent.PaymentProcessedFailed
                {
                    Id = Guid.NewGuid(),
                    BasketId = context.Message.BasketId,
                    UserId = context.Message.UserId,
                    OrderId = context.Message.OrderId,
                    Reason = "Timeout Payment",
                    TimeStamp = DateTime.Now,
                    TransactionId = context.Message.TransactionId
                };

                await context.Publish(paymentFailed, source.Token);
            }
        }
    }
}
