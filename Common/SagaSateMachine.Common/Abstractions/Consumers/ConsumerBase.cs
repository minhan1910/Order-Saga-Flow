using MassTransit;

namespace SagaSateMachine.Common.Abstractions.Consumers
{
    public abstract class ConsumerBase<T> : IConsumer<T>
        where T : class
    {
        public async Task Consume(ConsumeContext<T> context)
        {
            //try
            //{
                
            //}
            //catch (Exception)
            //{
            //    await context.Publish<Fault<T>>(context.Message);

            //    // global exception handling
            //    //throw;
            //}

            await ConsumeInternal(context);
        }

        protected abstract Task ConsumeInternal(ConsumeContext<T> context);
    }
}
