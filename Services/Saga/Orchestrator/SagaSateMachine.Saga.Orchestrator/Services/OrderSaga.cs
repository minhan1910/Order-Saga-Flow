using MassTransit;
using SagaSateMachine.Common.IntegrationEvents;
using SagaSateMachine.Saga.Orchestrator.States;

namespace SagaSateMachine.Saga.Orchestrator.Services;

public class OrderSaga : MassTransitStateMachine<OrderSagaState>
{
    public State OrderCreatedState { get; private set; }
    public State OrderCancelledState { get; private set; }
    public State OrderCreatedFaultedState { get; private set; }    
    public State OrderCompletedState { get; private set; }
    public State OrderFailedState { get; private set; }    
    public State BasketCheckoutCompletedState { get; private set; }    
    public State PaymentProcessedState { get; private set; }
    public State PaymentFailedState { get; private set; }
    public State PaymentProcessedFaultedState { get; private set; }    

    public Event<Command.BasketCheckout> BasketCheckoutStarted { get; private set; }
    public Event<DomainEvent.OrderCreated> OrderCreatedEvent { get; private set; }    
    public Event<DomainEvent.OrderCompleted> OrderCompletedEvent { get; private set; }
    public Event<DomainEvent.OrderCancelled> OrderCancelledEvent { get; private set; }    
    public Event<DomainEvent.PaymentProcessed> PaymentProcessedEvent { get; private set; }
    public Event<DomainEvent.PaymentProcessedFailed> PaymentProcessedFailedEvent { get; private set; }
    public Event<DomainEvent.OrderCreatedFailed> OrderCreatedFailedEvent { get; private set; }

    public OrderSaga()
    {
        InstanceState(x => x.CurrentState);

        Event(() => BasketCheckoutStarted, x => x.CorrelateById(context => context.Message.TransactionId));
        
        Event(() => OrderCreatedEvent, x => x.CorrelateById(context => context.Message.TransactionId));
        Event(() => OrderCompletedEvent, x => x.CorrelateById(context => context.Message.TransactionId));
        Event(() => OrderCancelledEvent, x => x.CorrelateById(context => context.Message.TransactionId));

        Event(() => PaymentProcessedEvent, x => x.CorrelateById(context => context.Message.TransactionId));

        Event(() => OrderCreatedFailedEvent, x => x.CorrelateById(context => context.Message.TransactionId));
        Event(() => PaymentProcessedFailedEvent, x => x.CorrelateById(context => context.Message.TransactionId));

        Initially(
            When(BasketCheckoutStarted)
                .Then(context =>
                {
                    context.Saga.IsBasketRemoved = false;
                    context.Saga.TransactionId = context.Message.TransactionId;
                    context.Saga.UserId = context.Message.UserId;
                    context.Saga.OrderId = Guid.NewGuid();
                })
                .Send(
                    context => new Uri("rabbitmq://localhost/create-order"), 
                    context => new Command.CreateOrder 
                    {
                        TransactionId = context.Saga.TransactionId,
                        OrderId = context.Saga.OrderId,
                        UserId = context.Message.UserId,
                        Items = context.Message.Items,
                        TotalPrice = context.Message.TotalPrice,
                        Id = context.Message.Id,
                        TimeStamp = context.Message.TimeStamp,
                    }
                )
                .TransitionTo(BasketCheckoutCompletedState)
        );

        During(BasketCheckoutCompletedState,
            When(OrderCreatedEvent)
                .Then(context => context.Saga.IsOrderCreated = true)
                .Send(
                    context => new Uri("rabbitmq://localhost/process-payment"),
                    context => new Command.ProcessPayment
                    {
                        Id = context.Message.Id,
                        PaymentId = Guid.NewGuid(),
                        UserId = context.Saga.UserId,
                        OrderId = context.Saga.OrderId,
                        TimeStamp = context.Message.TimeStamp,
                        TransactionId = context.Saga.TransactionId,
                    }
                )
                .TransitionTo(OrderCreatedState),
            When(OrderCreatedFailedEvent)
                .ThenAsync(async context =>
                {
                    if (!context.Saga.IsOrderCreated)
                    {
                        // send notification create order failed
                        await context.Send(
                            new Uri("rabbitmq://localhost/send-notification"),
                            new Command.SendNotifcation
                            {
                                UserId = context.Saga.UserId,
                                Message = "Create Order Failed. Please try again!",
                                TransactionId = context.Saga.TransactionId,
                            }
                        );
                    }
                })
                .Finalize()
        );

        During(OrderCreatedState,
            When(PaymentProcessedEvent)
                .ThenAsync(async context =>
                {
                    context.Saga.IsPaymentProcessed = true;
                    context.Saga.IsBasketRemoved = true;

                    await context.Send(
                        new Uri("rabbitmq://localhost/remove-basket"),
                        new Command.RemoveBasket
                        {
                            Id = Guid.NewGuid(),
                            BasketId = context.Message.BasketId,
                            OrderId = context.Message.OrderId,
                            UserId = context.Message.UserId,
                            TransactionId = context.Saga.TransactionId,
                            TimeStamp = DateTime.Now,
                        });
                })
                .TransitionTo(OrderCompletedState),
            When(PaymentProcessedFailedEvent)
                .ThenAsync(async context =>
                {
                    if (!context.Saga.IsPaymentCancelled)
                    {
                        await context.Send(
                            new Uri("rabbitmq://localhost/cancel-order"),
                            new Command.CancelOrder
                            {
                                Id = Guid.NewGuid(),
                                OrderId = context.Message.OrderId,
                                UserId = context.Message.UserId,
                                BasketId = context.Message.BasketId,
                                TransactionId = context.Saga.TransactionId,
                                TimeStamp = DateTime.Now,
                            }
                        );
                    }
                })
                .TransitionTo(PaymentFailedState)
        );

        During(OrderCompletedState,
            When(OrderCompletedEvent)
            .Send(
                context => new Uri("rabbitmq://localhost/send-notification"),
                context => new Command.SendNotifcation
                {
                    UserId = context.Saga.UserId,
                    Message = "Order and Payment processed successfully.",
                    TransactionId = context.Saga.TransactionId,
                })
            .Finalize()
        );

        #region Fault-Compensate State

        During(PaymentFailedState,
            When(OrderCancelledEvent)
            .ThenAsync(async context =>
            {
                context.Saga.IsPaymentCancelled = true;

                await context.Send(
                    new Uri("rabbitmq://localhost/send-notification"),
                    new Command.SendNotifcation
                    {
                        UserId = context.Message.UserId,
                        Message = "Cancel Order Successfuylly",
                        TransactionId = context.Saga.TransactionId,
                    });
            })
        );

        #endregion

        SetCompletedWhenFinalized();
    }
}
