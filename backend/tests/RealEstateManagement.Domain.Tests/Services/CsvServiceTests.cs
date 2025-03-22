using FluentAssertions;
using NSubstitute;
using RealEstateManagement.Domain.Services;
using System.Text;

namespace RealEstateManagement.Domain.Tests.Services
{
    public class CsvServiceTests
    {
        private readonly CsvService _csvService;

        public CsvServiceTests()
        {
            _csvService = new CsvService();
        }

        [Fact]
        public void ReadCSV_WhenValidCsv_ReturnsListOfRealEstateDto()
        {
            // Arrange
            var csvContent = "PropertyNumber;City\n123123;SP\n234234;RJ";
            var bytes = Encoding.Latin1.GetBytes(csvContent);
            using var stream = new MemoryStream(bytes);

            // Act
            var result = _csvService.ReadCSV(stream);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(2);
            result![0].PropertyNumber.Should().Be("123123");
            result![0].City.Should().Be("SP");
            result![1].PropertyNumber.Should().Be("234234");
            result![1].City.Should().Be("RJ");
        }

        [Fact]
        public void ReadCSV_WhenInvalidCsv_ReturnsNull()
        {
            // Arrange
            var invalidCsvContent = "Invalid;Format\nMissing;Columns";
            var bytes = Encoding.Latin1.GetBytes(invalidCsvContent);
            using var stream = new MemoryStream(bytes);

            // Act
            var result = _csvService.ReadCSV(stream);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public void ReadCSV_WhenEmptyCsv_ReturnsEmptyList()
        {
            // Arrange
            var emptyCsvContent = "PropertyNumber;City\n"; // Only headers, no data
            var bytes = Encoding.Latin1.GetBytes(emptyCsvContent);
            using var stream = new MemoryStream(bytes);

            // Act
            var result = _csvService.ReadCSV(stream);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();
        }

        [Fact]
        public void ReadCSV_WhenStreamIsInvalid_ReturnsNull()
        {
            // Arrange
            var stream = Substitute.For<Stream>();
            stream.When(x => x.Read(Arg.Any<byte[]>(), Arg.Any<int>(), Arg.Any<int>()))
                  .Do(x => throw new Exception("Stream error"));

            // Act
            var result = _csvService.ReadCSV(stream);

            // Assert
            result.Should().BeNull();
        }
    }

}
