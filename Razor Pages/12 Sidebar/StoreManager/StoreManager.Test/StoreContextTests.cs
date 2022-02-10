using StoreManager.Application.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace StoreManager.Test
{
    public class StoreContextTests : DatabaseTest
    {
        [Fact]
        public void EnsureCreatedSuccessTest()
        {
            _db.Database.EnsureCreated();
        }
        [Fact]
        public void SeedSuccessTest()
        {
            _db.Database.EnsureCreated();
            _db.Seed(new CryptService());

            _db.ChangeTracker.Clear();
            Assert.True(_db.Stores.ToList().Count > 0);
            Assert.True(_db.Products.ToList().Count > 0);
            Assert.True(_db.ProductCategories.ToList().Count > 0);
            Assert.True(_db.Offers.ToList().Count > 0);
        }
    }
}
