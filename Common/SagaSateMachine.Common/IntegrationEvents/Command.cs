using SagaSateMachine.Common.Abstractions.Messages;

namespace SagaSateMachine.Common.IntegrationEvents
{
    public static class Command
    {
        #region Basket
        public record BasketCheckout : IMessage
        {
            public Guid Id { get; set; }
            public Guid BasketId { get; set; }
            public Guid UserId { get; set; }            
            public Guid TransactionId { get; set; }

            public List<BasketItem> Items { get; set; } = new();
            public decimal TotalPrice { get; set; }
            public DateTimeOffset TimeStamp { get; set; }
        }

        public record BasketItem
        {
            public Guid CourseId { get; set; }
            public string? CourseName { get; set; }
            public decimal Price { get; set; }
        }

        public record RemoveBasket : IMessage
        {
            public Guid Id { get; set; }
            public Guid BasketId { get; set; }
            public Guid OrderId { get; set; }
            public Guid UserId { get; set; }
            public Guid TransactionId { get; set; }
            public DateTimeOffset TimeStamp { get; set; }
        }

        //public record RestoreBasket : IMessage
        //{
        //    public Guid Id { get; set; }
        //    public Guid BasketId { get; set; }
        //    public Guid UserId { get; set; }
        //    public Guid TransactionId { get; set; }
        //    public List<BasketItem> Items { get; set; } = new();
        //    public decimal TotalPrice { get; set; }
        //    public DateTimeOffset TimeStamp { get; set; }
        //}

        #endregion

        #region Order
        public record CreateOrder : IMessage
        {
            public Guid OrderId { get; set; }
            public Guid UserId { get; set; }
            public List<BasketItem> Items { get; set; }
            public decimal TotalPrice { get; set; }
            public Guid Id { get; set; }
            public DateTimeOffset TimeStamp { get; set; }
            public Guid TransactionId { get; set; }
        }

        public record CancelOrder : IMessage
        {
            public Guid TransactionId { get; set; }
            public Guid Id { get; set; }
            public Guid UserId { get; set; }
            public Guid OrderId { get; set; }
            public Guid BasketId { get; set; }
            public DateTimeOffset TimeStamp { get; set; }
        }

        #endregion

        #region Payment
        public record CancelPayment : IMessage
        {
            public Guid PaymentId { get; set; }
            public Guid OrderId { get; set; }
            public Guid Id { get; set; }
            public DateTimeOffset TimeStamp { get; set; }
            public Guid TransactionId { get; set; }
        }

        public class ProcessPayment : IMessage
        {
            public Guid Id { get; set; }
            public Guid PaymentId { get; set; }
            public Guid UserId { get; set; }
            public Guid OrderId { get; set; }
            public Guid BasketId { get; set; }
            public decimal TotalPrice { get; set; }
            public DateTimeOffset TimeStamp { get; set; }
            public Guid TransactionId { get; set; }
        }
        #endregion

        public record SendNotifcation
        {
            public Guid UserId { get; set; }
            public string? Message { get; set; }
            public Guid TransactionId { get; set; }
        }
    }
}
