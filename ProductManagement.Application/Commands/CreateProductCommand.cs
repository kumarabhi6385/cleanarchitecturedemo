using MediatR;

public class CreateProductCommand : IRequest<Guid>
{
    public required string Name { get; set; }
    public decimal Price { get; set; }
}
