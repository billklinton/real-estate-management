using RealEstateManagement.Shareable.Models;

namespace RealEstateManagement.Domain.Repositories
{
    public interface IUserRepository : IBaseRepository<User>
    {
        Task<bool> ValidateUserAsync(string email, string password);
    }
}
