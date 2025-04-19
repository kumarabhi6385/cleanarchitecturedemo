
using Microsoft.EntityFrameworkCore;
using Polly;

public class SqlProductRepository : IProductRepository
    {
        private readonly ProductDbContext _context;
        private readonly IAsyncPolicy _retryPolicy;
        private readonly IAsyncPolicy _circuitBreakerPolicy;

        public SqlProductRepository(ProductDbContext context, IAsyncPolicy retryPolicy, IAsyncPolicy circuitBreakerPolicy)
        {
            _context = context;
            _retryPolicy = retryPolicy;
            _circuitBreakerPolicy = circuitBreakerPolicy;
        }

    public async Task<Product> GetByIdAsync(Guid id)
    {
        return await _retryPolicy.ExecuteAsync(async () =>
        {
            return await _circuitBreakerPolicy.ExecuteAsync(async () =>
            {
                return await _context.Products.FindAsync(id);
            });
        });
    }
    public async Task<IEnumerable<Product>> GetAllAsync() => await _context.Products.ToListAsync();
        public async Task AddAsync(Product product)
        {
            await _retryPolicy.ExecuteAsync(async () =>
            {
                await _circuitBreakerPolicy.ExecuteAsync(async () =>
                {
                    _context.Products.Add(product);
                    await _context.SaveChangesAsync();
                });
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
