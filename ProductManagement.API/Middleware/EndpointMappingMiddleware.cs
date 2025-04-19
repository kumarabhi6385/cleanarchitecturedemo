using MediatR;

public static class EndpointMappingMiddleware
{
    public static void MapEndpoints(this WebApplication app)
    {
        app.MapPost("/api/products", async (CreateProductCommand command, IMediator mediator) =>
        {
            var productId = await mediator.Send(command);
            return Results.Ok(productId);
        });

        app.MapGet("/api/products/{id}", async (Guid id, IMediator mediator) =>
        {
            var product = await mediator.Send(new GetProductQuery { Id = id });
            return product is not null ? Results.Ok(product) : Results.NotFound();
        });
                
        app.MapGet("/api/heartbeat", () => Results.Ok("I am still alive"));
    }
}
