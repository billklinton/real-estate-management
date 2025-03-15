using Confluent.Kafka;
using RealEstateManagement.Shareable.Configs;
using System.Text.Json;

namespace RealEstateManagement.Kafka
{
    public class Producer : IProducer
    {
        private readonly IProducer<string, string> _producer;
        private readonly string _topicName;

        public Producer(AppConfig appConfig)
        {
            _topicName = appConfig.KafkaConfig.TopicName;

            var config = new ProducerConfig
            {
                BootstrapServers = appConfig.KafkaConfig.BootstrapServers
            };

            _producer = new ProducerBuilder<string, string>(config).Build();
        }

        public async Task SendMessageAsync<T>(T message)
        {
            var msg = new Message<string, string>
            {
                Key = Guid.NewGuid().ToString(),
                Value = JsonSerializer.Serialize(message)
            };

            var deliveryResult = await _producer.ProduceAsync(_topicName, msg);
            Console.WriteLine($"Sent Message:'{deliveryResult.Value}'. Topic {deliveryResult.TopicPartitionOffset}. Status:{deliveryResult.Status}");
        }
    }
}
