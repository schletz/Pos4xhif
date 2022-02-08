using Bogus;
using Microsoft.EntityFrameworkCore;
using StoreManager.Application.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoreManager.Application.Infrastructure
{
    public class StoreContext : DbContext
    {
        public StoreContext(DbContextOptions opt) : base(opt) { }
        public DbSet<PriceTrend> PriceTrends => Set<PriceTrend>();
        public DbSet<User> Users => Set<User>();
        public DbSet<Store> Stores => Set<Store>();
        public DbSet<Offer> Offers => Set<Offer>();
        public DbSet<Product> Products => Set<Product>();
        public DbSet<ProductCategory> ProductCategories => Set<ProductCategory>();
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Offer>().Property(o => o.Price).HasPrecision(9, 4);
            modelBuilder.Entity<Offer>().HasIndex(o => new { o.StoreId, o.ProductEan }).IsUnique();
            foreach (var entity in modelBuilder.Model.GetEntityTypes())
            {
                var type = entity.ClrType;
                if (type.GetProperty("Guid") is not null)
                    modelBuilder.Entity(type).HasAlternateKey("Guid");
            }
        }

        public void Initialize(CryptService cryptService, string adminPassword)
        {
            var adminSalt = cryptService.GenerateSecret(256);
            var admin = new User(
                username: "admin",
                salt: adminSalt,
                passwordHash: cryptService.GenerateHash(adminSalt, adminPassword),
                usertype: Usertype.Admin);
            Users.Add(admin);
            SaveChanges();
        }

        public void Seed(ICryptService cryptService)
        {
            Randomizer.Seed = new Random(1938);
            var faker = new Faker("de");

            var i = 0;
            var stores = new Faker<Store>("de").CustomInstantiator(f =>
            {
                var name = f.Company.CompanyName();
                var salt = cryptService.GenerateSecret(256);
                var username = $"store{++i:000}";
                return new Store(
                    name: f.Company.CompanyName(),
                    closeDate: new DateTime(2021, 1, 1).AddDays(f.Random.Int(0, 365)).OrNull(f, 0.5f),
                    url: f.Internet.Url().OrDefault(f, 0.25f),
                    manager: new User(
                        username: username,
                        salt: salt,
                        passwordHash: cryptService.GenerateHash(salt, "1234"),
                        usertype: Usertype.Owner));
            })
            .Generate(10)
            .GroupBy(s => s.Name).Select(g => g.First())
            .ToList();
            Stores.AddRange(stores);
            SaveChanges();

            var productCategories = new Faker<ProductCategory>("de").CustomInstantiator(f =>
            {
                return new ProductCategory(
                    name: f.Commerce.ProductAdjective(),
                    nameEn: f.Commerce.ProductAdjective().OrDefault(f, 0.5f));
            })
            .Generate(10)
            .GroupBy(p => p.Name).Select(g => g.First())
            .ToList();
            ProductCategories.AddRange(productCategories);
            SaveChanges();

            var products = new Faker<Product>("de").CustomInstantiator(f =>
            {
                return new Product(
                    ean: f.Random.Int(100000, 999999),
                    name: f.Commerce.ProductName(),
                    productCategory: f.Random.ListItem(productCategories),
                    recommendedPrice: (f.Random.Int(1000, 100000) / 100M).OrNull(f, 0.3f),
                    availableFrom: new DateTime(2022, 1, 1).AddDays(f.Random.Int(0, 300)).OrNull(f, 0.8f)
                    );
            })
            .Generate(100)
            .GroupBy(p => p.Ean).Select(g => g.First())
            .ToList();
            Products.AddRange(products);
            SaveChanges();

            var offers = new Faker<Offer>("de").CustomInstantiator(f =>
            {
                var store = f.Random.ListItem(stores);
                var lastUpdate = (store.CloseDate ?? new DateTime(2022, 1, 1)).AddDays(-f.Random.Int(0, 365));
                return new Offer(
                    product: f.Random.ListItem(products),
                    store: store,
                    price: f.Random.Int(1000, 99900) / 100M,
                    soldOut: f.Random.Bool(0.25f),
                    lastUpdate: lastUpdate);
            })
            .Generate(500)
            .GroupBy(o => new { o.StoreId, o.ProductEan }).Select(g => g.First())
            .ToList();
            Offers.AddRange(offers);
            SaveChanges();

            // Tägliche Preise generieren. Dabei wird eine Schwankung von bis zu 5 %
            // alle 20 - 40 Tage in die Tabelle priceTrends geschrieben.
            var priceTrends = new List<PriceTrend>();
            foreach (var offer in offers)
            {
                var start = new DateTime(2021, 12, 31);
                var end = new DateTime(2021, 1, 1);
                var priceCent = (int)(offer.Price * 100);
                for (var date = start; date >= end; date = date.AddDays(-faker.Random.Int(20, 40)))
                {
                    priceTrends.Add(new PriceTrend(
                        offer: offer, date: date, price: priceCent / 100M));
                    var diff = (int)(priceCent * 0.05);
                    priceCent = faker.Random.Int(priceCent - diff, priceCent + diff);
                }
            }
            PriceTrends.AddRange(priceTrends);
            SaveChanges();
        }
    }
}
