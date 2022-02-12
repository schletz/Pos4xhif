using CsvHelper;
using CsvHelper.Configuration;
using ExcelDataReader;
using Microsoft.EntityFrameworkCore;
using StoreManager.Application.Infrastructure;
using StoreManager.Application.Model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoreManager.Application.Services
{
    public class ProductImportService
    {
        /// <summary>
        /// Represents a CSV record. Open setters are required
        /// if we use Mappers in CSVHelper.
        /// </summary>
        private class CsvRow
        {
            public int Ean { get; set; }
            public string Name { get; set; } = default!;
            public string ProductCategory { get; set; } = default!;
            public decimal? RecommendedPrice { get; set; }
            public DateTime? AvailableFrom { get; set; }
        }
        private class NotNullStringConverter : CsvHelper.TypeConversion.StringConverter
        {
            public override object ConvertFromString(string text, IReaderRow row, MemberMapData memberMapData)
            {
                if (string.IsNullOrEmpty(text)) { throw new CsvHelperException(row.Context); }
                return base.ConvertFromString(text, row, memberMapData);
            }
        }
        private class CsvRowMap : ClassMap<CsvRow>
        {
            public CsvRowMap()
            {
                Map(row => row.Ean).Index(0);   // 1st column is ean number
                Map(row => row.Name).Name("Produkt").TypeConverter<NotNullStringConverter>();
                Map(row => row.ProductCategory).Name("Kategorie").TypeConverter<NotNullStringConverter>();
                Map(row => row.RecommendedPrice).Convert(args =>
                {
                    string? val = args.Row["UVP"];
                    if (string.IsNullOrEmpty(val)) { return null; }
                    return decimal.Parse(val, CultureInfo.InvariantCulture);
                });
                Map(row => row.AvailableFrom).Convert(args =>
                {
                    string? val = args.Row["LieferbarAb"];
                    if (string.IsNullOrEmpty(val)) { return null; }
                    return DateTime.ParseExact(val, "dd.MM.yyyy", CultureInfo.InvariantCulture);
                });
            }
        }

        private readonly StoreContext _db;
        public ProductImportService(StoreContext db)
        {
            _db = db;
        }

        public (bool success, string message) LoadCsv(Stream stream)
        {
            var csvConfig = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = true,
                Delimiter = "\t",
                NewLine = "\r\n",
                ReadingExceptionOccurred = (context) => false       // true: rethrow exception, false: ignore
            };
            using var reader = new StreamReader(stream, new UTF8Encoding(false));
            using var csv = new CsvReader(reader, csvConfig);
            csv.Context.RegisterClassMap<CsvRowMap>();
            try
            {
                var records = csv.GetRecords<CsvRow>().ToList();
                return WriteToDatabase(records);
            }
            catch (CsvHelperException ex)
            {
                return (false, $"Fehler beim Lesen der Zeile {ex.Context.Parser.Row}: {ex.Message}");
            }
        }

        public (bool success, string message) LoadExcel(Stream stream, int maxRows = 1000)
        {
            // Requires
            // <PackageReference Include="System.Text.Encoding.CodePages" Version="6.*" />
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            // <PackageReference Include="ExcelDataReader" Version="3.*" />
            using var reader = ExcelReaderFactory.CreateReader(stream);
            reader.Read();  // Ignore header
            var csvRows = new List<CsvRow>(1024);
            int rowNumber = 0;
            while (reader.Read() && rowNumber++ < maxRows)
            {
                if (reader.FieldCount < 5) { break; }
                if (reader.IsDBNull(0)) { break; }
                try
                {
                    csvRows.Add(new CsvRow
                    {
                        Ean = reader.IsDBNull(0) ? throw new ApplicationException("Invalid EAN") : (int)reader.GetDouble(0),
                        Name = reader.IsDBNull(1) ? throw new ApplicationException("Invalid product name") : reader.GetString(1),
                        ProductCategory = reader.IsDBNull(2) ? throw new ApplicationException("Invalid product category") : reader.GetString(2),
                        RecommendedPrice = reader.IsDBNull(3) ? null : (decimal)reader.GetDouble(3),
                        AvailableFrom = reader.IsDBNull(4) ? null : reader.GetDateTime(4)
                    });
                }
                catch { }  // Ignore invalid rows
            }
            return WriteToDatabase(csvRows);
        }


        private (bool success, string message) WriteToDatabase(IEnumerable<CsvRow> csvRows)
        {
            var existingEans = _db.Products.Select(p => p.Ean).ToHashSet();
            var existingCategories = _db.ProductCategories.Select(p => p.Name).ToHashSet();

            var newCategories = csvRows
                .Where(c => !existingCategories.Contains(c.ProductCategory))
                .GroupBy(c => c.ProductCategory)
                .Select(g => new ProductCategory(name: g.Key));
            _db.ProductCategories.AddRange(newCategories);
            try
            {
                _db.SaveChanges();
            }
            catch (DbUpdateException ex)
            {
                return (false, ex.InnerException?.Message ?? ex.Message);
            }

            var categories = _db.ProductCategories.ToDictionary(c => c.Name, c => c);
            var newProducts = csvRows
                .Where(r => !existingEans.Contains(r.Ean))
                .Select(r => new Product(
                    ean: r.Ean,
                    name: r.Name,
                    productCategory: categories[r.ProductCategory],
                    recommendedPrice: r.RecommendedPrice,
                    availableFrom: r.AvailableFrom));
            _db.Products.AddRange(newProducts);
            try
            {
                var count = _db.SaveChanges();
                return (true, $"{count} Produkte importiert.");
            }
            catch (DbUpdateException ex)
            {
                return (false, ex.InnerException?.Message ?? ex.Message);
            }
        }
    }
}
