using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoreManager.Application.Model
{
    [Table("Product")]
    public class Product : IEntity<int>
    {
        public Product(int ean, string name, ProductCategory productCategory)
        {
            Ean = ean;
            Name = name;
            ProductCategoryId = productCategory.Id;
            ProductCategory = productCategory;
            Guid = Guid.NewGuid();
        }
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        protected Product() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        // Ean is the PK and not an auto increment column. Annotations are used
        // for the next property (ean)
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Ean { get; private set; }                  // private set, EF Core does not support changing the PK.
        public int Id => Ean;                                 // To implement the interface
        public string Name { get; set; }
        public int ProductCategoryId { get; set; }            // Value of the FK
        public ProductCategory ProductCategory { get; set; }  // Navigation property
        // Navigation to the offers of the product. Read-only (get) because this is not a mapped property for the database.
        public Guid Guid { get; set; }
        public ICollection<Offer> Offers { get; } = new List<Offer>();
    }
}
