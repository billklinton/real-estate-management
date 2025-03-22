using Confluent.Kafka;
using RealEstateManagement.Domain.Mappers;
using RealEstateManagement.Domain.Repositories;
using RealEstateManagement.Kafka;
using RealEstateManagement.Shareable.Dtos;

namespace RealEstateManagement.Domain.Services
{
    public class ConsumerService : IConsumerService<List<RealEstateDto>>
    {
        private readonly IRealEstateRepository _realEstateRepository;

        public ConsumerService(IRealEstateRepository realEstateRepository)
        {
            _realEstateRepository = realEstateRepository;
        }

        public async Task ConsumeAsync(IConsumer<string, List<RealEstateDto>> consumer, CancellationToken cancellationToken)
        {
            ConsumeResult<string, List<RealEstateDto>> consumeResult = null;

            try
            {
                consumeResult = consumer.Consume(cancellationToken);

                if (consumeResult?.Message?.Value != null)
                {
                    var realStateList = RealEstateMapper.ToListModel(consumeResult.Message.Value);
                    await _realEstateRepository.InserManyAsync(realStateList);
                }
            }
            catch
            {
            }
        }
    }
}
