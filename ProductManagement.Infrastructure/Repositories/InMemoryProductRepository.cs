
public class InMemoryProductRepository : IProductRepository
    {
        private readonly List<Product> _products = new();

        public Task<Product> GetByIdAsync(Guid id) => Task.FromResult(_products.FirstOrDefault(p => p.Id == id));
        public Task<IEnumerable<Product>> GetAllAsync() => Task.FromResult(_products.AsEnumerable());
        public Task AddAsync(Product product)
        {
            _products.Add(product);
            return Task.CompletedTask;
        }
        public Task UpdateAsync(Product product)
        {
            var existingProduct = _products.FirstOrDefault(p => p.Id == product.Id);
            if (existingProduct != null)
            {
                existingProduct.Name = product.Name;
                existingProduct.Price = product.Price;
            }
            return Task.CompletedTask;
        }
        public Task DeleteAsync(Guid id)
        {
            var product = _products.FirstOrDefault(p => p.Id == id);
            if (product != null)
            {
                _products.Remove(product);
            }
            return Task.CompletedTask;
        }
    }
