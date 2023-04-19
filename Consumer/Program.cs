using Common;
using Confluent.Kafka;
using Consumer;
using MassTransit;

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
                rider.AddConsumer<MessageConsumer>();
                rider.AddConsumer<FaultMessageConsumer>();
                rider.UsingKafka((context, k) =>
                {
                    k.Host("localhost:29092");
                    k.TopicEndpoint<Message>("test-topic", "test-consumer-group", e =>
                    {
                        e.CreateIfMissing(t => t.NumPartitions = 1);
                        //e.AutoOffsetReset = AutoOffsetReset.Latest;
                        e.ConfigureConsumer<MessageConsumer>(context);
                        e.ConfigureConsumer<FaultMessageConsumer>(context);

                        e.UseMessageRetry(r => r.Interval(3, TimeSpan.FromSeconds(3)));
                    });
                });
            });
        });
    })
    .Build();

host.Run();
