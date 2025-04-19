
using MediatR;

public class GetProductQuery : IRequest<Product>
    {
        public Guid Id { get; set; }
    }
