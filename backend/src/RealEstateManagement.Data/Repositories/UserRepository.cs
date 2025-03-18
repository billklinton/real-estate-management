using MongoDB.Driver;
using RealEstateManagement.Domain.Repositories;
using RealEstateManagement.Shareable.Models;

namespace RealEstateManagement.Data.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly IMongoCollection<User> _collection;

        public UserRepository(IMongoCollection<User> collection)
        {
            _collection = collection;
        }

        public async Task<bool> ValidateUserAsync(string email, string password)
        {
            var usr = await _collection.Find(u => u.Email == email && u.PasswordHash == password).FirstOrDefaultAsync();

            return usr != null;
        }

        public Task InserManyAsync(List<User> entity)
        {
            throw new NotImplementedException();
        }
    }
}
