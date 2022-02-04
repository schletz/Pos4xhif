using StoreManager.Application.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace StoreManager.Application.Infrastructure.Repositories
{
    public class StoreRepository : Repository<Store, int>
    {
        public record StoreWithOffersCount(
            Guid Guid,
            string Name,
            DateTime? CloseDate,
            string? Url,
            int OffersCount);
        public StoreRepository(StoreContext db) : base(db) { }

        public IReadOnlyList<StoreWithOffersCount> GetStoresWithOffersCount()
        {
            return _db.Stores
                .Select(s => new StoreWithOffersCount(s.Guid, s.Name, s.CloseDate, s.Url, s.Offers.Count()))
                .ToList();
        }

        public override (bool success, string message) Delete(Store store)
        {
            if (!store.CloseDate.HasValue) { return (false, "Der Store ist nicht geschlossen."); }
            return base.Delete(store);
        }
    }
}
