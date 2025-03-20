using MediatR;
using OperationResult;
using RealEstateManagement.Domain.Mappers;
using RealEstateManagement.Domain.Repositories;
using RealEstateManagement.Shareable.Dtos;
using RealEstateManagement.Shareable.Exceptions;
using RealEstateManagement.Shareable.Requests;
using RealEstateManagement.Shareable.Responses;

namespace RealEstateManagement.Domain.Handlers
{
    public class GetByIdRequestHandler : IRequestHandler<GetByIdRequest, Result<BaseResponse<RealEstateDto>>>
    {
        private readonly IRealEstateRepository _realEstateRepository;

        public GetByIdRequestHandler(IRealEstateRepository realEstateRepository)
        {
            _realEstateRepository = realEstateRepository;
        }

        public async Task<Result<BaseResponse<RealEstateDto>>> Handle(GetByIdRequest request, CancellationToken cancellationToken)
        {
            var realEstate = await _realEstateRepository.GetByIdAsync(request.Id);

            if (realEstate is null)
                return new NotFoundException("Real Estate not found");

            return Result.Success(new BaseResponse<RealEstateDto>(200, "Success", RealEstateMapper.ToDto(realEstate)));
        }
    }
}
