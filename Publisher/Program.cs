using Common;
using Confluent.Kafka;
using MassTransit;
using Publisher;
using IHost = Microsoft.Extensions.Hosting.IHost;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddHostedService<Worker>();
        services.AddMassTransit(x =>
        {
            x.UsingInMemory((context, config) => config.ConfigureEndpoints(context));
            //x.UsingRabbitMq((context, cfg) => cfg.ConfigureEndpoints(context));
            x.AddRider(rider =>
            {
                rider.AddProducer<Null, Message>("test-topic", (riderContext, producerConfig) =>
                {
                });
                rider.UsingKafka((context, k) =>
                {
                    k.Host("localhost:29092");
                });
            });
        });
    })
    .Build();
var a = host.Services.GetRequiredService<IBusControl>();
a.Start();

host.Run();
