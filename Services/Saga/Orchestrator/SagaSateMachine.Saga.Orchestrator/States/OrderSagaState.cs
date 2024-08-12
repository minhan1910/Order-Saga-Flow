using MassTransit;

namespace SagaSateMachine.Saga.Orchestrator.States
{
    public class OrderSagaState : SagaStateMachineInstance
    {
        public Guid CorrelationId { get; set; }
        public Guid TransactionId { get; set; }
        public Guid OrderId { get; set; }
        public Guid UserId { get; set; }
        public string CurrentState { get; set; }

        public bool IsBasketRemoved { get; set; }
        public bool IsOrderCreated { get; set; }
        public bool IsPaymentProcessed { get; set; }
        public bool IsNotificationSent { get; set; }
        public bool IsOrderCancelled { get; set; }
        public bool IsPaymentCancelled { get; set; }
    }
}
