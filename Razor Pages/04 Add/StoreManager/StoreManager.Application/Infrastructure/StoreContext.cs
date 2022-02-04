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
        public DbSet<Store> Stores => Set<Store>();
        public DbSet<Offer> Offers => Set<Offer>();
        public DbSet<Product> Products => Set<Product>();
        public DbSet<ProductCategory> ProductCategories => Set<ProductCategory>();
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Offer>().Property(o => o.Price).HasPrecision(9, 4);
            modelBuilder.Entity<Offer>().HasIndex(o => new { o.StoreId, o.ProductEan }).IsUnique();
            modelBuilder.Entity<Store>().HasAlternateKey(o => o.Guid);
        }

        public void Seed()
        {
            Randomizer.Seed = new Random(1938);

            var stores = new Faker<Store>("de").CustomInstantiator(f =>
            {
                return new Store(
                    name: f.Company.CompanyName(),
                    closeDate: new DateTime(2021, 1, 1).AddDays(f.Random.Int(0, 365)).OrNull(f, 0.25f),
                    url: f.Internet.Url().OrDefault(f, 0.25f));
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
                    productCategory: f.Random.ListItem(productCategories));
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
                    lastUpdate: lastUpdate);
            })
            .Generate(500)
            .GroupBy(o => new { o.StoreId, o.ProductEan }).Select(g => g.First())
            .ToList();
            Offers.AddRange(offers);
            SaveChanges();
        }
    }
}
