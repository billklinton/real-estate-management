using MediatR;
using Microsoft.IdentityModel.Tokens;
using OperationResult;
using RealEstateManagement.Domain.Repositories;
using RealEstateManagement.Shareable.Configs;
using RealEstateManagement.Shareable.Exceptions;
using RealEstateManagement.Shareable.Requests;
using RealEstateManagement.Shareable.Responses;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace RealEstateManagement.Domain.Handlers
{
    public class LoginRequestHandler : IRequestHandler<LoginRequest, Result<TokenResponse>>
    {
        private readonly AppConfig _appConfig;
        private readonly IUserRepository _userRepository;

        public LoginRequestHandler(AppConfig appConfig, IUserRepository userRepository)
        {
            _appConfig = appConfig;
            _userRepository = userRepository;
        }

        public async Task<Result<TokenResponse>> Handle(LoginRequest request, CancellationToken cancellationToken)
        {
            var validated = await _userRepository.ValidateUserAsync(request.Email, request.Password);

            if (!validated)
                throw new UnauthorizedException("Invalid user credentials!");

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appConfig.TokenConfig.JWTKey);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity([new Claim(ClaimTypes.Email, request.Email)]),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return Result.Success(new TokenResponse(tokenHandler.WriteToken(token)));
        }
    }
}
