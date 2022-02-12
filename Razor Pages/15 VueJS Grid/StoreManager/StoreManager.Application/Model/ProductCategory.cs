using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoreManager.Application.Model
{
    [Table("ProductCategory")]
    public class ProductCategory : IEntity<int>
    {
        public ProductCategory(string name, string? nameEn = null)
        {
            Name = name;
            NameEn = nameEn;
            Guid = Guid.NewGuid();
        }
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        protected ProductCategory() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public int Id { get; private set; }
        public string Name { get; set; }
        public string? NameEn { get; set; }       // Optional (nullable reference types). Can be set by initializor.
        public Guid Guid { get; private set; }
    }
}
