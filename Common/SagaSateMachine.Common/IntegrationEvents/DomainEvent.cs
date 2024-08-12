using SagaSateMachine.Common.Abstractions.Messages;

namespace SagaSateMachine.Common.IntegrationEvents
{
    public static class DomainEvent
    {
        public class OrderCreated : IMessage
        {
            public Guid UserId { get; set; }
            public Guid OrderId { get; set; }
            public Guid Id { get; set; }
            public DateTimeOffset TimeStamp { get; set; }
            public Guid TransactionId { get; set; }
        }

        public class OrderCompleted : IMessage
        {
            public Guid BasketId { get; set; }
            public Guid OrderId { get; set; }
            public Guid UserId { get; set; }
            public string Message { get; set; }
            public Guid Id { get; set; }
            public DateTimeOffset TimeStamp { get; set; }
            public Guid TransactionId { get; set; }
        }

        public class OrderCancelled : IMessage
        {
            public Guid UserId { get; set; }
            public Guid Id { get; set; }
            public DateTimeOffset TimeStamp { get; set; }
            public Guid TransactionId { get; set; }
        }

        public class PaymentCancelled : IMessage
        {
            public Guid PaymentId { get; set; }
            public Guid OrderId { get; set; }
            public Guid UserId { get; set; }
            public Guid Id { get; set; }
            public DateTimeOffset TimeStamp { get; set; }
            public Guid TransactionId { get; set; }
        }

        public class PaymentProcessed : IMessage
        {
            public Guid PaymentId { get; set; }
            public Guid OrderId { get; set; }            
            public Guid UserId { get; set; }            
            public Guid BasketId { get; set; }            
            public Guid Id { get; set; }
            public DateTimeOffset TimeStamp { get; set; }
            public Guid TransactionId { get; set; }
        }

        public class PaymentProcessedFailed : IMessage
        {
            public Guid Id { get; set; }
            public Guid OrderId { get; set; }
            public Guid UserId { get; set; }
            public Guid BasketId { get; set; }
            public Guid TransactionId { get; set; }
            public string Reason { get; set; }
            public DateTimeOffset TimeStamp { get; set; }
        }

        public class OrderCreatedFailed : IMessage
        {
            public Guid Id { get; set; }            
            public Guid UserId { get; set; }
            public string? Reason { get; set; }
            public DateTimeOffset TimeStamp { get; set; }
            public Guid TransactionId { get; set; }
        }
    }
}
