using RealEstateManagement.Domain.Services;
using RealEstateManagement.IoC;
using RealEstateManagement.Kafka;
using RealEstateManagement.Shareable.Dtos;
using RealEstateManagement.Worker;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.ConfigureWorkerAppDependencies(builder.Configuration);
builder.Services.ConfigureDatabase(builder.Configuration);

builder.Services.AddScoped<IConsumerService<List<RealEstateDto>>, ConsumerService>();
builder.Services.AddHostedService<WorkerConsumer>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

Endpoints.MapEndpoints(app);

app.Run();
