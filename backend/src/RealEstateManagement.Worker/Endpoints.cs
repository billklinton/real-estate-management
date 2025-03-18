using System.Text;

namespace RealEstateManagement.Worker
{
    public static class Endpoints
    {
        public static void MapEndpoints(this IEndpointRouteBuilder app)
        {
            app.MapGet("/", async context =>
            {
                context.Response.ContentType = "text/text;charset=UTF-8";
                await context.Response.WriteAsync("Worker em execução", Encoding.UTF8);
            });
        }
    }
}
