using FluentAssertions;
using NSubstitute;
using NSubstitute.ReturnsExtensions;
using OperationResult;
using RealEstateManagement.Domain.Handlers;
using RealEstateManagement.Domain.Repositories;
using RealEstateManagement.Shareable.Dtos;
using RealEstateManagement.Shareable.Models;
using RealEstateManagement.Shareable.Requests;
using RealEstateManagement.Shareable.Requests.Validations;
using RealEstateManagement.Shareable.Responses;

namespace RealEstateManagement.Domain.Tests.Handlers
{
    public class GetByIdRequestHandlerTests
    {
        private readonly GetByIdRequestHandler _sut;
        private readonly IRealEstateRepository _realEstateRepository = Substitute.For<IRealEstateRepository>();
        public GetByIdRequestHandlerTests()
        {
            _sut = new GetByIdRequestHandler(_realEstateRepository);
        }

        [Fact]
        public async Task GetByIdResquest_WhenExists_ReturnsSuccess()
        {
            //Arrange
            var id = new Guid();
            var request = new GetByIdRequest(id);
            var response = new RealEstate
            {
                PropertyNumber = "12345",
                City = "SP"
            };
            _realEstateRepository.GetByIdAsync(id).Returns(response);

            //Act
            var (success, result, _) = await _sut.Handle(request, CancellationToken.None);

            //Assert
            success.Should().BeTrue();
            result!.StatusCode.Should().Be(200);
            result.Message.Should().Be("Success");
            result.Result.PropertyNumber.Should().Be(response.PropertyNumber);
            result.Result.City.Should().Be(response.City);
            await _realEstateRepository.Received().GetByIdAsync(Arg.Is<Guid>(x => x == id));
        }

        [Fact]
        public async Task GetByIdResquest_WhenDontExists_ReturnsError()
        {
            //Arrange
            var id = new Guid();
            var request = new GetByIdRequest(id);
            _realEstateRepository.GetByIdAsync(id).ReturnsNull();

            //Act
            var (success, _, error) = await _sut.Handle(request, CancellationToken.None);

            //Assert
            success.Should().BeFalse();
            error!.Message.Should().Be("Real Estate not found");
            await _realEstateRepository.Received().GetByIdAsync(Arg.Is<Guid>(x => x == id));
        }
    }
}
