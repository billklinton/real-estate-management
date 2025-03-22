using FluentAssertions;
using NSubstitute;
using RealEstateManagement.Domain.Handlers;
using RealEstateManagement.Domain.Repositories;
using RealEstateManagement.Shareable.Configs;
using RealEstateManagement.Shareable.Models;
using RealEstateManagement.Shareable.Requests;

namespace RealEstateManagement.Domain.Tests.Handlers
{
    public class LoginRequestHandlerTests
    {
        private readonly LoginRequestHandler _sut;
        private readonly IUserRepository _userRepository = Substitute.For<IUserRepository>();

        public LoginRequestHandlerTests()
        {
            _sut = new LoginRequestHandler(new AppConfig() { TokenConfig = new() { JWTKey = "c453f28f0dabf168a7f29a3d649cdec619c26aca0b7824dd7d4f015553004fda" } } , _userRepository);
        }

        [Fact]
        public async Task LoginResquest_WhenValidUser_ReturnsSuccess()
        {
            //Arrange
            var request = new LoginRequest("teste@teste.com", "password");
            _userRepository.ValidateUserAsync(request.Email, request.Password)
                                 .Returns(true);

            //Act
            var (success, result, _) = await _sut.Handle(request, CancellationToken.None);

            //Assert
            success.Should().BeTrue();
            await _userRepository.Received().ValidateUserAsync(request.Email, request.Password);
        }

        [Fact]
        public async Task LoginResquest_WhenInvalidUser_ReturnsError()
        {
            //Arrange
            var request = new LoginRequest("teste@teste.com", "password");
            _userRepository.ValidateUserAsync(request.Email, request.Password).Returns(false);

            //Act
            var (success, _, error) = await _sut.Handle(request, CancellationToken.None);

            //Assert
            success.Should().BeFalse();
            error!.Message.Should().Be("Invalid user credentials!");
            await _userRepository.Received().ValidateUserAsync(request.Email, request.Password);
        }
    }
}
