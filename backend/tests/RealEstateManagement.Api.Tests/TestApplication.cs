using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSubstitute;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace RealEstateManagement.Api.Tests;

public class TestApplication : WebApplicationFactory<Program>
{
    private readonly string _environment;

    public TestApplication(string env = "Development")
        => _environment = env;

    protected override IHost CreateHost(IHostBuilder builder)
    {
        var mediator = Substitute.For<IMediator>();
        
        builder.ConfigureServices(services =>
        {
            services.AddTransient(_ => mediator);

            services.AddAuthentication("TestBearer")
                .AddScheme<AuthenticationSchemeOptions, MockAuthenticationHandler>("TestBearer", options => { });

            services.AddAuthorization(options =>
            {
                options.DefaultPolicy = new AuthorizationPolicyBuilder()
                    .AddAuthenticationSchemes("TestBearer")
                    .RequireAuthenticatedUser()
                    .Build();
            });
        });
        builder.UseEnvironment(_environment);
        return base.CreateHost(builder);
    }

    public class MockAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        public MockAuthenticationHandler(IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder)
            : base(options, logger, encoder)
        {
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var claims = new[] { new Claim(ClaimTypes.Email, "teste@teste.com") };
            var identity = new ClaimsIdentity(claims, "TestBearer");
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, "TestBearer");

            return Task.FromResult(AuthenticateResult.Success(ticket));
        }
    }
}