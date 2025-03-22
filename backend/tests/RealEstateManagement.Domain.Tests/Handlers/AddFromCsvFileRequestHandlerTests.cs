using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using RealEstateManagement.Domain.Handlers;
using RealEstateManagement.Domain.Services;
using RealEstateManagement.Kafka;
using RealEstateManagement.Shareable.Dtos;
using RealEstateManagement.Shareable.Requests;
using System.Text;

namespace RealEstateManagement.Domain.Tests.Handlers
{
    public class AddFromCsvFileRequestHandlerTests
    {
        private readonly AddFromCsvFileRequestHandler _sut;
        private readonly ICsvService _csvService = Substitute.For<ICsvService>();
        private readonly IProducer _producer = Substitute.For<IProducer>();

        public AddFromCsvFileRequestHandlerTests()
        {
            _sut = new AddFromCsvFileRequestHandler(_producer, _csvService);
        }

        [Fact]
        public async Task AddFromCsvFileRequest_WhenValidCsv_ProcessesSuccessfully()
        {
            // Arrange
            var realEstates = new List<RealEstateDto>
            {
                new RealEstateDto { PropertyNumber = "123123", City = "SP" },
                new RealEstateDto { PropertyNumber = "234234", City = "RJ" }
            };

            var content = "valid,csv,content";
            var bytes = Encoding.UTF8.GetBytes(content);
            var stream = new MemoryStream(bytes);
            IFormFile file = new FormFile(stream, 0, stream.Length, "file", "test.csv")
            {
                Headers = new HeaderDictionary(),
                ContentType = "text/csv"
            };
            IFormFileCollection fileCollection = new FormFileCollection { file };
            var request = new AddFromCsvFileRequest(fileCollection);

            _csvService.ReadCSV(Arg.Any<Stream>()).Returns(realEstates);

            // Act
            var (success, result, _) = await _sut.Handle(request, CancellationToken.None);

            // Assert
            success.Should().BeTrue();
            result!.StatusCode.Should().Be(200);
            result.Message.Should().Be("All real estate records have been successfully processed.");
            result.Result.Should().Be("Success");
            await _producer.Received().SendMessageAsync(Arg.Is<List<RealEstateDto>>(x => x.First().PropertyNumber == realEstates.First().PropertyNumber));
        }

        [Fact]
        public async Task AddFromCsvFileRequest_WhenLessThanBatchSize_SendsAllAtOnce()
        {
            // Arrange
            var realEstates = Enumerable.Range(1, 500).Select(i => new RealEstateDto
            {
                PropertyNumber = i.ToString(),
                City = "SP"
            }).ToList();

            var content = "valid,csv,content";
            var bytes = Encoding.UTF8.GetBytes(content);
            var stream = new MemoryStream(bytes);
            IFormFile file = new FormFile(stream, 0, stream.Length, "file", "test.csv")
            {
                Headers = new HeaderDictionary(),
                ContentType = "text/csv"
            };
            IFormFileCollection fileCollection = new FormFileCollection { file };
            var request = new AddFromCsvFileRequest(fileCollection);

            _csvService.ReadCSV(Arg.Any<Stream>()).Returns(realEstates);

            // Act
            var (success, result, _) = await _sut.Handle(request, CancellationToken.None);

            // Assert
            success.Should().BeTrue();
            result!.StatusCode.Should().Be(200);
            result.Message.Should().Be("All real estate records have been successfully processed.");
            result.Result.Should().Be("Success");
            await _producer.Received().SendMessageAsync(Arg.Is<List<RealEstateDto>>(x => x.Count == realEstates.Count));
        }

        [Fact]
        public async Task AddFromCsvFileRequest_WhenExactlyBatchSize_SendsOneBatch()
        {
            // Arrange
            var realEstates = Enumerable.Range(1, 1000).Select(i => new RealEstateDto
            {
                PropertyNumber = i.ToString(),
                City = "SP"
            }).ToList();

            var content = "valid,csv,content";
            var bytes = Encoding.UTF8.GetBytes(content);
            var stream = new MemoryStream(bytes);
            IFormFile file = new FormFile(stream, 0, stream.Length, "file", "test.csv")
            {
                Headers = new HeaderDictionary(),
                ContentType = "text/csv"
            };
            IFormFileCollection fileCollection = new FormFileCollection { file };
            var request = new AddFromCsvFileRequest(fileCollection);

            _csvService.ReadCSV(Arg.Any<Stream>()).Returns(realEstates);

            // Act
            var (success, result, _) = await _sut.Handle(request, CancellationToken.None);

            // Assert
            success.Should().BeTrue();
            result!.StatusCode.Should().Be(200);
            result.Message.Should().Be("All real estate records have been successfully processed.");
            result.Result.Should().Be("Success");
            await _producer.Received(1).SendMessageAsync(Arg.Any<List<RealEstateDto>>());
        }

        [Fact]
        public async Task AddFromCsvFileRequest_WhenMoreThanOneBatch_SendsMultipleBatches()
        {
            // Arrange
            var realEstates = Enumerable.Range(1, 2500).Select(i => new RealEstateDto
            {
                PropertyNumber = i.ToString(),
                City = "SP"
            }).ToList();

            var content = "valid,csv,content";
            var bytes = Encoding.UTF8.GetBytes(content);
            var stream = new MemoryStream(bytes);
            IFormFile file = new FormFile(stream, 0, stream.Length, "file", "test.csv")
            {
                Headers = new HeaderDictionary(),
                ContentType = "text/csv"
            };
            IFormFileCollection fileCollection = new FormFileCollection { file };
            var request = new AddFromCsvFileRequest(fileCollection);

            _csvService.ReadCSV(Arg.Any<Stream>()).Returns(realEstates);

            // Act
            var (success, result, _) = await _sut.Handle(request, CancellationToken.None);

            // Assert
            success.Should().BeTrue();
            result!.StatusCode.Should().Be(200);
            result.Message.Should().Be("All real estate records have been successfully processed.");
            result.Result.Should().Be("Success");
            await _producer.Received(3).SendMessageAsync(Arg.Any<List<RealEstateDto>>());
        }

        [Fact]
        public async Task AddFromCsvFileRequest_WhenBatchSizeExceeded_SendsFinalBatch()
        {
            // Arrange
            var realEstates = Enumerable.Range(1, 1100).Select(i => new RealEstateDto
            {
                PropertyNumber = i.ToString(),
                City = "SP"
            }).ToList();

            var content = "valid,csv,content";
            var bytes = Encoding.UTF8.GetBytes(content);
            var stream = new MemoryStream(bytes);
            IFormFile file = new FormFile(stream, 0, stream.Length, "file", "test.csv")
            {
                Headers = new HeaderDictionary(),
                ContentType = "text/csv"
            };
            IFormFileCollection fileCollection = new FormFileCollection { file };
            var request = new AddFromCsvFileRequest(fileCollection);

            _csvService.ReadCSV(Arg.Any<Stream>()).Returns(realEstates);

            // Act
            var (success, result, _) = await _sut.Handle(request, CancellationToken.None);

            // Assert
            success.Should().BeTrue();
            result!.StatusCode.Should().Be(200);
            result.Message.Should().Be("All real estate records have been successfully processed.");
            result.Result.Should().Be("Success");
            await _producer.Received(2).SendMessageAsync(Arg.Any<List<RealEstateDto>>());
        }

        [Fact]
        public async Task AddFromCsvFileRequest_WhenInvalidCsv_ReturnsError()
        {
            //Arrange
            var content = "dummy,csv,content";
            var bytes = Encoding.UTF8.GetBytes(content);
            var stream = new MemoryStream(bytes);
            IFormFile file = new FormFile(stream, 0, stream.Length, "file", "test.csv")
            {
                Headers = new HeaderDictionary(),
                ContentType = "text/csv"
            };
            IFormFileCollection fileCollection = new FormFileCollection { file };
            var request = new AddFromCsvFileRequest(fileCollection);

            _csvService.ReadCSV(request.Files[0].OpenReadStream()).ReturnsNull();

            //Act
            var (success, _, error) = await _sut.Handle(request, CancellationToken.None);

            //Assert
            success.Should().BeFalse();
            error?.Message.Should().Be("Error processing the given file");
            _csvService.Received().ReadCSV(Arg.Is<Stream>(x => x.Length == request.Files[0].OpenReadStream().Length));
            _producer.DidNotReceive();
        }
    }
}
