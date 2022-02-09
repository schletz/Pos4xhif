using StoreManager.Application.Model;

namespace StoreManager.Application.Infrastructure.Repositories
{
    public class PriceTrendRepository : Repository<PriceTrend, int>
    {
        public PriceTrendRepository(StoreContext db) : base(db) { }
    }

}
