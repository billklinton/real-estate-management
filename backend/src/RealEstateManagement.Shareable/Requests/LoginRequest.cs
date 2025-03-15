using MediatR;
using OperationResult;
using RealEstateManagement.Shareable.Dtos;
using RealEstateManagement.Shareable.Responses;

namespace RealEstateManagement.Shareable.Requests
{
    public record LoginRequest(string Email, string Password) : IRequest<Result<TokenResponse>>
    {
    }
}
