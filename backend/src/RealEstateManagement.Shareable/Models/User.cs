using MongoDB.Bson;

namespace RealEstateManagement.Shareable.Models
{
    public class User
    {
        public ObjectId Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
    }
}
