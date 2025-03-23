using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using RealEstateManagement.Shareable.Requests;
using RealEstateManagement.Shareable.Responses;
using FluentAssertions;
using System.Text.Json;
using System.Text;
using System.Net;
using RealEstateManagement.Shareable.Exceptions;
using System.Net.Http.Headers;
using RealEstateManagement.Shareable.Dtos;
using System.Net.Http.Json;

namespace RealEstateManagement.Api.Tests;

public class EndpointsTests
{
    private static readonly TestApplication _testApplication = new();
    private static readonly HttpClient _client = _testApplication.CreateClient();
    private static readonly IMediator _mediator = _testApplication.Services.GetRequiredService<IMediator>();

    public EndpointsTests()
    {
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("TestBearer", "mock-token");
    }

    [Fact]
    public async Task AddFromCsvFileRequest_WhenValid_ReturnsOk()
    {
        // Arrange
        var response = new BaseResponse<string>(200, "All real estate records have been successfully processed.", "Success");
        _mediator.Send(Arg.Any<AddFromCsvFileRequest>()).Returns(response);

        var content = "dummy,csv,content";
        var bytes = Encoding.UTF8.GetBytes(content);
        using var httpContent = new MultipartFormDataContent
        {
            { new ByteArrayContent(bytes) { Headers = { ContentType = new MediaTypeHeaderValue("text/csv") } }, "Files", "test.csv" }
        };

        // Act
        var result = await _client.PostAsync("api/v1/add-from-csvfile", httpContent);

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.OK);
        await _mediator.Received().Send(Arg.Any<AddFromCsvFileRequest>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task AddFromCsvFileRequest_WhenInvalid_ReturnsBadRequest()
    {
        // Arrange
        var request = new AddFromCsvFileRequest(default!);
        var errors = new Dictionary<string, IEnumerable<string>>
        {
            { "Files", new List<string> { "At least one file must be uploaded" } }
        };
        var ex = new DataInvalidException(errors);

        _mediator.Send(Arg.Any<AddFromCsvFileRequest>()).Returns(ex);

        var content = "dummy,csv,content";
        var bytes = Encoding.UTF8.GetBytes(content);
        using var httpContent = new MultipartFormDataContent
        {
            { new ByteArrayContent(bytes) { Headers = { ContentType = new MediaTypeHeaderValue("text/csv") } }, "Files", "test.csv" }
        };        

        // Act
        var result = await _client.PostAsync("api/v1/add-from-csvfile", httpContent);

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var msg = await result.Content.ReadAsStringAsync();
        msg.Should().ContainAny("At least one file must be uploaded");
        await _mediator.Received().Send(Arg.Any<AddFromCsvFileRequest>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task LoginRequest_WhenValid_ReturnsOk()
    {
        // Arrange
        var loginRequest = new LoginRequest("test@example.com", "password");
        var response = new TokenResponse("werwerwrwerwrwrewrwerwerwer");

        _mediator.Send(loginRequest).Returns(response);

        var httpContent = new StringContent(JsonSerializer.Serialize(loginRequest), Encoding.UTF8, "application/json");

        // Act
        var result = await _client.PostAsync("api/v1/login", httpContent);

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.OK);
        await _mediator.Received().Send(Arg.Is<LoginRequest>(x => x.Email == loginRequest.Email && x.Password == loginRequest.Password));
    }

    [Fact]
    public async Task LoginRequest_WhenInvalid_ReturnsBadRequest()
    {
        // Arrange
        var loginRequest = new LoginRequest(default!, default!);

        var errors = new Dictionary<string, IEnumerable<string>>
        {
            { "Email", new List<string> { "Email can't be empty or invalid" } },
            { "Password", new List<string> { "Password can't be empty" } }
        };
        var ex = new DataInvalidException(errors);

        _mediator.Send(loginRequest).Returns(ex);

        var httpContent = new StringContent(JsonSerializer.Serialize(loginRequest), Encoding.UTF8, "application/json");

        // Act
        var result = await _client.PostAsync("api/v1/login", httpContent);

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var msg = await result.Content.ReadAsStringAsync();
        msg.Should().ContainAny("Email can't be empty or invalid");
        msg.Should().ContainAny("Password can't be empty");
        await _mediator.Received().Send(Arg.Is<LoginRequest>(x => x.Email == loginRequest.Email && x.Password == loginRequest.Password));
    }

    [Fact]
    public async Task LoginRequest_WhenUser_IsUnauthorizedOrDoesNotExists_ReturnsUnauthorized()
    {
        // Arrange
        var loginRequest = new LoginRequest("test@example.com", "password");

        var exception = new UnauthorizedException();
        _mediator.Send(loginRequest).Returns(exception);

        var httpContent = new StringContent(JsonSerializer.Serialize(loginRequest), Encoding.UTF8, "application/json");

        // Act
        var result = await _client.PostAsync("api/v1/login", httpContent);

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        var response = await result.Content.ReadFromJsonAsync<BaseResponse<string>>();
        response!.Message.Should().Be("Invalid user credentials!");
        await _mediator.Received().Send(Arg.Is<LoginRequest>(x => x.Email == loginRequest.Email && x.Password == loginRequest.Password));
    }

    [Fact]
    public async Task GetByIdRequest_WhenValid_ReturnsOk()
    {
        // Arrange
        var id = new Guid();
        var getByIdRequest = new GetByIdRequest(id);
        var realEstateDto = new RealEstateDto
        {
            PropertyNumber = "12312124123",
            City = "SP"
        };
        var response = new BaseResponse<RealEstateDto>(200, "Success", realEstateDto);

        _mediator.Send(getByIdRequest).Returns(response);

        // Act
        var result = await _client.GetAsync($"api/v1/getById?id={id}");

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.OK);
        var realEstateDtoResponse = await result.Content.ReadFromJsonAsync<BaseResponse<RealEstateDto>>();
        realEstateDtoResponse!.Result.PropertyNumber.Should().Be(realEstateDto.PropertyNumber);
        realEstateDtoResponse!.Result.City.Should().Be(realEstateDto.City);
        await _mediator.Received().Send(Arg.Is<GetByIdRequest>(x => x.Id == id));
    }

    [Fact]
    public async Task GetByIdRequest_WhenId_IsNotProvided_ReturnsBadRequest()
    {
        // Arrange
        var getByIdRequest = new GetByIdRequest(default!);

        var errors = new Dictionary<string, IEnumerable<string>>
        {
            { "Id", new List<string> { "Id can't be empty" } }
        };
        var ex = new DataInvalidException(errors);

        _mediator.Send(getByIdRequest).Returns(ex);

        // Act
        var result = await _client.GetAsync($"api/v1/getById");

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var msg = await result.Content.ReadAsStringAsync();
        msg.Should().ContainAny("Id can't be empty");
    }

    [Fact]
    public async Task GetByIdRequest_WhenId_DoesNotExists_ReturnsNotFound()
    {
        // Arrange
        var id = new Guid();
        var getByIdRequest = new GetByIdRequest(id);

        var exception = new NotFoundException("Real Estate not found");
        _mediator.Send(getByIdRequest).Returns(exception);

        // Act
        var result = await _client.GetAsync($"api/v1/getById?id={id}");

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.NotFound);
        var response = await result.Content.ReadFromJsonAsync<BaseResponse<string>>();
        response!.Message.Should().Be("Real Estate not found");
        await _mediator.Received().Send(Arg.Is<GetByIdRequest>(x => x.Id == id));
    }

    [Fact]
    public async Task GetRequest_WhenValid_ReturnsOk()
    {
        // Arrange
        var getRequest = new GetRealEstateRequest(0, 2, "SP", "São Paulo", "Direct");
        
        var realEstateList = new List<RealEstateDto>
        {
            new()
            {
                PropertyNumber = "12312124123",
                City = "SP"
            },
            new()
            {
                PropertyNumber = "12312124123",
                City = "SP"
            }
        };
        var response = new BaseResponse<List<RealEstateDto>>(200, "Success", realEstateList);

        _mediator.Send(getRequest).Returns(response);

        // Act
        var result = await _client.GetAsync($"api/v1/get?state={getRequest.State}&city={getRequest.City}&page={getRequest.Page}&pageSize={getRequest.PageSize}&saleMode={getRequest.SaleMode}");

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.OK);
        var realEstateDtoResponse = await result.Content.ReadFromJsonAsync<BaseResponse<List<RealEstateDto>>>();
        realEstateDtoResponse!.Message.Should().Be("Success");
        realEstateDtoResponse!.Result.Should().HaveCount(2);
        await _mediator.Received().Send(Arg.Is<GetRealEstateRequest>(x => x.Page == getRequest.Page &&
                                                                           x.PageSize == getRequest.PageSize &&
                                                                           x.State == getRequest.State &&
                                                                           x.City == getRequest.City &&
                                                                           x.SaleMode == getRequest.SaleMode));


    }

    [Fact]
    public async Task GetRequest_WhenInvalidDataRequest_ReturnsBadRequest()
    {
        // Arrange
        var getRequest = new GetRealEstateRequest(-1, -1);

        var errors = new Dictionary<string, IEnumerable<string>>
        {
            { "Page", new List<string> { "Page must be greater than or equal to 0" } },
            { "PageSize", new List<string> { "PageSize must be between 1 and 100" } }
        };
        var exception = new DataInvalidException(errors);

        _mediator.Send(getRequest).Returns(exception);

        // Act
        var result = await _client.GetAsync($"api/v1/get?page={getRequest.Page}&pageSize={getRequest.PageSize}");

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var msg = await result.Content.ReadAsStringAsync();
        msg.Should().ContainAny("Page must be greater than or equal to 0");
        msg.Should().ContainAny("PageSize must be between 1 and 100");
        await _mediator.Received().Send(Arg.Is<GetRealEstateRequest>(x => x.Page == getRequest.Page &&
                                                                           x.PageSize == getRequest.PageSize &&
                                                                           x.State == getRequest.State &&
                                                                           x.City == getRequest.City &&
                                                                           x.SaleMode == getRequest.SaleMode));
    }

    [Fact]
    public async Task GetRequest_WhenDoesNotExists_ReturnsNotFound()
    {
        // Arrange
        var getRequest = new GetRealEstateRequest(0, 1, "IN");
        var exception = new NotFoundException("Real Estates were not found");

        _mediator.Send(getRequest).Returns(exception);

        // Act
        var result = await _client.GetAsync($"api/v1/get?page={getRequest.Page}&pageSize={getRequest.PageSize}&state={getRequest.State}");

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.NotFound);
        var response = await result.Content.ReadFromJsonAsync<BaseResponse<string>>();
        response!.Message.Should().Be("Real Estates were not found");
        await _mediator.Received().Send(Arg.Is<GetRealEstateRequest>(x => x.Page == getRequest.Page &&
                                                                           x.PageSize == getRequest.PageSize &&
                                                                           x.State == getRequest.State &&
                                                                           x.City == getRequest.City &&
                                                                           x.SaleMode == getRequest.SaleMode));
    }

    [Fact]
    public async Task WhenValidUrl_ReturnsNotFound()
    {
        // Act
        var result = await _client.GetAsync("");

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}