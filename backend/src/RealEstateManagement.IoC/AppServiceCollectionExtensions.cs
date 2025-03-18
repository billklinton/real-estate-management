using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using RealEstateManagement.Data.Repositories;
using RealEstateManagement.Domain;
using RealEstateManagement.Domain.Repositories;
using RealEstateManagement.Domain.Services;
using RealEstateManagement.Kafka;
using RealEstateManagement.Shareable.Configs;
using RealEstateManagement.Shareable.Models;

namespace RealEstateManagement.IoC
{
    public static class AppServiceCollectionExtensions
    {
        public static void ConfigureAppDependencies(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddMediatR(cfg => {
                cfg.RegisterServicesFromAssembly(typeof(IDomainEntryPoint).Assembly);
            });

            AppConfig appConfig = new();
            configuration.Bind(appConfig);
            configuration.GetSection("KafkaConfig").Bind(appConfig.KafkaConfig);
            configuration.GetSection("TokenConfig").Bind(appConfig.TokenConfig);

            services.AddSingleton(appConfig);
        }

        public static void ConfigureWorkerAppDependencies(this IServiceCollection services, IConfiguration configuration)
        {
            AppConfig appConfig = new();
            configuration.Bind(appConfig);
            configuration.GetSection("KafkaConfig").Bind(appConfig.KafkaConfig);

            services.AddSingleton(appConfig);
        }

        public static void ConfigureServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IProducer, Producer>();
            services.AddScoped<ICsvService, CsvService>();
        }

        public static void ConfigureDatabase(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<MongoDbSettings>(configuration.GetSection("MongoDbSettings"));

            services.AddSingleton(s => new MongoClient(configuration["MongoDbSettings:ConnectionString"]));

            var database = services.BuildServiceProvider().GetRequiredService<MongoClient>().GetDatabase(configuration["MongoDbSettings:DatabaseName"]);
            services.AddSingleton(sp => database.GetCollection<User>("User"));
            services.AddSingleton(sp => database.GetCollection<RealEstate>("RealEstate"));

            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IRealEstateRepository, RealEstateRepository>();
        }
    }
}
