using StoreManager.Application.Model;

namespace StoreManager.Application.Infrastructure.Repositories
{
    public class ProductRepository : Repository<Product, int>
    {
        public ProductRepository(StoreContext db) : base(db) { }
    }
}
