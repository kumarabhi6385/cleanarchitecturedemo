
using Microsoft.EntityFrameworkCore;
using Polly;

public class SqlProductRepository : IProductRepository
{
    private readonly ProductDbContext _context;
    private readonly IAsyncPolicy _resiliencePolicy;

    public SqlProductRepository(ProductDbContext context, IAsyncPolicy resiliencePolicy)
    {
        _context = context;
        _resiliencePolicy = resiliencePolicy;
    }

    public async Task<Product?> GetByIdAsync(Guid id)
    {
        var product = await _resiliencePolicy.ExecuteAsync(async () =>
        {
            return await _context.Products.FindAsync(id);
        });

        // product will be null if not found, which is expected
        return product;
    }
    public async Task<IEnumerable<Product>> GetAllAsync() => await _context.Products.ToListAsync();
    public async Task AddAsync(Product product)
    {
        await _resiliencePolicy.ExecuteAsync(async () =>
        {
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
        });
    }
    public async Task UpdateAsync(Product product)
    {
        _context.Products.Update(product);
        await _context.SaveChangesAsync();
    }
    public async Task DeleteAsync(Guid id)
    {
        var product = await _context.Products.FindAsync(id);
        if (product != null)
        {
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
        }
    }
}
