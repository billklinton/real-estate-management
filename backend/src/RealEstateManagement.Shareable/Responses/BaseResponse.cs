namespace RealEstateManagement.Shareable.Responses
{
    public record BaseResponse<T>(int StatusCode, string Message, T Result = default!);
}
