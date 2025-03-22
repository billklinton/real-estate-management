using Confluent.Kafka;
using FluentAssertions;
using NSubstitute;
using RealEstateManagement.Domain.Repositories;
using RealEstateManagement.Domain.Services;
using RealEstateManagement.Shareable.Dtos;
using RealEstateManagement.Shareable.Models;

namespace RealEstateManagement.Domain.Tests.Services
{
    public class ConsumerServiceTests
    {
        private readonly IRealEstateRepository _realEstateRepository;
        private readonly IConsumer<string, List<RealEstateDto>> _consumer;
        private readonly ConsumerService _consumerService;

        public ConsumerServiceTests()
        {
            _realEstateRepository = Substitute.For<IRealEstateRepository>();
            _consumer = Substitute.For<IConsumer<string, List<RealEstateDto>>>();
            _consumerService = new ConsumerService(_realEstateRepository);
        }

        [Fact]
        public async Task ConsumeAsync_WhenMessageIsValid_InsertsIntoRepository()
        {
            // Arrange
            var realEstates = new List<RealEstateDto>
            {
                new RealEstateDto { PropertyNumber = "123123", City = "SP" },
                new RealEstateDto { PropertyNumber = "234234", City = "RJ" }
            };

            var consumeResult = new ConsumeResult<string, List<RealEstateDto>>
            {
                Message = new Message<string, List<RealEstateDto>> { Value = realEstates }
            };

            _consumer.Consume(Arg.Any<CancellationToken>()).Returns(consumeResult);

            var mappedRealEstates = new List<RealEstate>
            {
                new RealEstate { PropertyNumber = "123123", City = "SP" },
                new RealEstate { PropertyNumber = "234234", City = "RJ" }
            };

            // Act
            await _consumerService.ConsumeAsync(_consumer, CancellationToken.None);

            // Assert
            await _realEstateRepository.Received(1).InserManyAsync(Arg.Is<List<RealEstate>>(list =>
                list.Count == mappedRealEstates.Count &&
                list[0].PropertyNumber == mappedRealEstates[0].PropertyNumber &&
                list[1].PropertyNumber == mappedRealEstates[1].PropertyNumber
            ));
        }

        [Fact]
        public async Task ConsumeAsync_WhenMessageIsNull_DoesNotInsertIntoRepository()
        {
            // Arrange
            var consumeResult = new ConsumeResult<string, List<RealEstateDto>>
            {
                Message = null
            };

            _consumer.Consume(Arg.Any<CancellationToken>()).Returns(consumeResult);

            // Act
            await _consumerService.ConsumeAsync(_consumer, CancellationToken.None);

            // Assert
            await _realEstateRepository.DidNotReceive().InserManyAsync(Arg.Any<List<RealEstate>>());
        }

        [Fact]
        public async Task ConsumeAsync_WhenMessageValueIsNull_DoesNotInsertIntoRepository()
        {
            // Arrange
            var consumeResult = new ConsumeResult<string, List<RealEstateDto>>
            {
                Message = new Message<string, List<RealEstateDto>> { Value = null }
            };

            _consumer.Consume(Arg.Any<CancellationToken>()).Returns(consumeResult);

            // Act
            await _consumerService.ConsumeAsync(_consumer, CancellationToken.None);

            // Assert
            await _realEstateRepository.DidNotReceive().InserManyAsync(Arg.Any<List<RealEstate>>());
        }

        [Fact]
        public async Task ConsumeAsync_WhenExceptionIsThrown_DoesNotThrowException()
        {
            // Arrange
            _consumer.Consume(Arg.Any<CancellationToken>()).Returns(x => throw new Exception("Consumer error"));

            // Act & Assert (No exception should be thrown)
            Func<Task> act = async () => await _consumerService.ConsumeAsync(_consumer, CancellationToken.None);
            await act.Should().NotThrowAsync();
        }
    }

}
