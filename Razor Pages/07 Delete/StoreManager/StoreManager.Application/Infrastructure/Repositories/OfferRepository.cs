using StoreManager.Application.Model;
using System;
using System.Linq;

namespace StoreManager.Application.Infrastructure.Repositories
{
    public class OfferRepository : Repository<Offer, int>
    {
        public OfferRepository(StoreContext db) : base(db) { }
        public override (bool success, string message) Insert(Offer offer)
        {
            offer.LastUpdate = DateTime.UtcNow;
            return base.Insert(offer);
        }
        public (bool success, string message) Insert(decimal price, bool soldOut, Guid productGuid, Guid storeGuid)
        {
            var product = _db.Products.FirstOrDefault(p => p.Guid == productGuid);
            if (product is null) { return (false, "Ungültiges Produkt."); }
            var store = _db.Stores.FirstOrDefault(p => p.Guid == storeGuid);
            if (store is null) { return (false, "Ungültiger Store."); }
            return base.Insert(new Offer(
                product: product,
                store: store,
                price: price,
                lastUpdate: DateTime.UtcNow,
                soldOut: soldOut));
        }
    }
}
