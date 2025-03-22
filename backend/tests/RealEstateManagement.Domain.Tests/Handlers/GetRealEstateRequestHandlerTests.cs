using FluentAssertions;
using NSubstitute;
using RealEstateManagement.Domain.Handlers;
using RealEstateManagement.Domain.Repositories;
using RealEstateManagement.Shareable.Models;
using RealEstateManagement.Shareable.Requests;

namespace RealEstateManagement.Domain.Tests.Handlers
{
    public class GetRealEstateRequestHandlerTests
    {
        private readonly GetRealEstateRequestHandler _sut;
        private readonly IRealEstateRepository _realEstateRepository = Substitute.For<IRealEstateRepository>();

        public GetRealEstateRequestHandlerTests()
        {
            _sut = new GetRealEstateRequestHandler(_realEstateRepository);
        }

        [Fact]
        public async Task GetResquest_WhenExists_ReturnsSuccess()
        {
            //Arrange
            var request = new GetRealEstateRequest(0, 2, "SP", "São Paulo", "Direct");

            var response = new List<RealEstate>
            {
                new()
                {
                    PropertyNumber = "12312124123",
                    City = "SP"
                },
                new()
                {
                    PropertyNumber = "12312124123",
                    City = "SP"
                }
            };
            _realEstateRepository.GetAsync(request.Page, request.PageSize, request.State, request.City, request.SaleMode)
                                 .Returns(response);

            //Act
            var (success, result, _) = await _sut.Handle(request, CancellationToken.None);

            //Assert
            success.Should().BeTrue();
            result!.StatusCode.Should().Be(200);
            result.Message.Should().Be("Success");
            await _realEstateRepository.Received().GetAsync(request.Page, request.PageSize, request.State, request.City, request.SaleMode);
        }

        [Fact]
        public async Task GetResquest_WhenDontExists_ReturnsError()
        {
            //Arrange
            var request = new GetRealEstateRequest(0, 2, "SP", "São Paulo", "Direct");

            _realEstateRepository.GetAsync(request.Page, request.PageSize, request.State, request.City, request.SaleMode)
                                 .Returns([]);

            //Act
            var (success, _, error) = await _sut.Handle(request, CancellationToken.None);

            //Assert
            success.Should().BeFalse();
            error!.Message.Should().Be("Real Estates were not found");
            await _realEstateRepository.Received().GetAsync(request.Page, request.PageSize, request.State, request.City, request.SaleMode);
        }
    }
}
