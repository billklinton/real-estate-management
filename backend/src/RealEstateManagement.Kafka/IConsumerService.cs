using Confluent.Kafka;
using RealEstateManagement.Shareable.Dtos;

namespace RealEstateManagement.Kafka
{
    public interface IConsumerService<T>
    {
        Task ConsumeAsync(IConsumer<string, List<RealEstateDto>> consumer, CancellationToken cancellationToken);
    }
}
