using RealEstateManagement.Shareable.Dtos;
using RealEstateManagement.Shareable.Models;

namespace RealEstateManagement.Domain.Mappers
{
    public static class RealEstateMapper
    {
        public static RealEstate ToModel(RealEstateDto realEstateDto) =>
            new()
            {
                Id = Guid.CreateVersion7(),
                AccessLink = realEstateDto.AccessLink,
                Address = realEstateDto.Address,
                AppraisalValue = realEstateDto.AppraisalValue,
                City = realEstateDto.City,
                Description = realEstateDto.Description,
                Discount = realEstateDto.Discount,
                Neighborhood = realEstateDto.Neighborhood,
                Price = realEstateDto.Price,
                PropertyNumber = realEstateDto.PropertyNumber,
                SaleMode = realEstateDto.SaleMode,
                State = realEstateDto.State
            };

        public static List<RealEstate> ToListModel(List<RealEstateDto> realEstateDtoList) =>
            realEstateDtoList.Select(ToModel).ToList();
    }
}
