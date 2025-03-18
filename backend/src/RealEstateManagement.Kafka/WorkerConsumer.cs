using Confluent.Kafka;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RealEstateManagement.Shareable.Configs;
using RealEstateManagement.Shareable.Dtos;

namespace RealEstateManagement.Kafka
{
    public class WorkerConsumer : BackgroundService
    {
        private readonly IServiceScope _scope;
        private readonly IConsumer<string, List<RealEstateDto>> _consumer;
        private readonly AppConfig _appConfig;

        public WorkerConsumer(IServiceProvider serviceProvider, AppConfig appConfig)
        {
            _appConfig = appConfig;
            _scope = serviceProvider.CreateScope();
            _consumer = new ConsumerBuilder<string, List<RealEstateDto>>(
                new ConsumerConfig
                {
                    BootstrapServers = appConfig.KafkaConfig.BootstrapServers,
                    GroupId = appConfig.KafkaConfig.GroupId
                })
                .SetValueDeserializer(new ListDeserializer<RealEstateDto>())
                .Build();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _consumer.Subscribe(_appConfig.KafkaConfig.TopicName);

            var consumerService = _scope.ServiceProvider.GetRequiredService<IConsumerService<List<RealEstateDto>>>();

            while (!stoppingToken.IsCancellationRequested)
                await consumerService.ConsumeAsync(_consumer, stoppingToken);

            _consumer.Close();
        }
    }
}
