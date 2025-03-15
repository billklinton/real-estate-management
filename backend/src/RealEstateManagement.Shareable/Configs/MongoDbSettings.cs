using Microsoft.Extensions.Configuration;

namespace RealEstateManagement.Shareable.Configs
{
    public class MongoDbSettings
    {
        public string ConnectionString { get; set; } = default!;
        public string DatabaseName { get; set; } = default!;
    }
}
