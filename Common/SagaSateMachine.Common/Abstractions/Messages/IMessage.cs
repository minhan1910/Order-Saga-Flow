using MassTransit;

namespace SagaSateMachine.Common.Abstractions.Messages;

[ExcludeFromTopology]
public interface IMessage
{
    public Guid Id { get; set; }
    public DateTimeOffset TimeStamp { get; set; }
}
