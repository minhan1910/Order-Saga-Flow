using MassTransit;
using SagaSateMachine.Common.Abstractions.Consumers;
using SagaSateMachine.Common.IntegrationEvents;

namespace SagaSateMachine.NotificationService.Consumers
{
    public class SendNotificationConsumer : ConsumerBase<Command.SendNotifcation>
    {
        private readonly ILogger<SendNotificationConsumer> _logger;

        public SendNotificationConsumer(ILogger<SendNotificationConsumer> logger)
        {
            _logger = logger;
        }

        protected override async Task ConsumeInternal(ConsumeContext<Command.SendNotifcation> context)
        {
            _logger.LogInformation("SendNotificationConsumer: {message}", context.Message);

            await Task.CompletedTask;
        }
    }
}
