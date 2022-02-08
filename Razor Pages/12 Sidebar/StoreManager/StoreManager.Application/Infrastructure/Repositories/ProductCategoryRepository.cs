using StoreManager.Application.Model;

namespace StoreManager.Application.Infrastructure.Repositories
{
    public class ProductCategoryRepository : Repository<ProductCategory, int>
    {
        public ProductCategoryRepository(StoreContext db) : base(db) { }
    }
}
