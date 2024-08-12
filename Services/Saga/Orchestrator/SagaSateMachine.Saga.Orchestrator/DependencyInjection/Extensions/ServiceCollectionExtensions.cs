using MassTransit;
using SagaSateMachine.Saga.Orchestrator.DependencyInjection.Options;
using SagaSateMachine.Saga.Orchestrator.Services;
using SagaSateMachine.Saga.Orchestrator.States;
using System.Reflection;

namespace SagaSateMachine.Saga.Orchestrator.DependencyInjection.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddConfigureMasstransitRabbit(this IServiceCollection services, IConfiguration configuration)
    {
        var masstransitConfiguration = new MasstransitConfiguration();
        configuration.GetSection(nameof(MasstransitConfiguration)).Bind(masstransitConfiguration);

        services.AddMassTransit(cfg =>
        {
            cfg.AddSagaStateMachine<OrderSaga, OrderSagaState>()
                .InMemoryRepository();

            cfg.AddConsumers(Assembly.GetExecutingAssembly());

            cfg.SetKebabCaseEndpointNameFormatter();

            cfg.UsingRabbitMq((context, bus) =>
            {
                bus.Host(masstransitConfiguration.Host, masstransitConfiguration.VHost, h =>
                {
                    h.Username(masstransitConfiguration.UserName);
                    h.Password(masstransitConfiguration.Password);
                });

                bus.MessageTopology.SetEntityNameFormatter(new KebabCaseEntityNameFormatter());

                bus.ConfigureEndpoints(context);
            });

        });

        return services;
    }
}
