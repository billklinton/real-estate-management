using RealEstateManagement.Shareable.Models;

namespace RealEstateManagement.Domain.Repositories
{
    public interface IRealEstateRepository : IBaseRepository<RealEstate>
    {
        Task<List<RealEstate>> GetAsync(int page, int pageSize, string? state = null, string? city = null, string? saleMode = null);
        Task<RealEstate> GetByIdAsync(Guid id);
    }
}
