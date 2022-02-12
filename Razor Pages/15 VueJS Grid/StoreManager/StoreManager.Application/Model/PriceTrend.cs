using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoreManager.Application.Model
{
    /// <summary>
    /// Price history. A read-only table for previous prices of offers.
    /// </summary>
    [Table("PriceTrend")]
    public class PriceTrend : IEntity<int>
    {
        public PriceTrend(Offer offer, DateTime date, decimal price)
        {
            Offer = offer;
            Date = date;
            Price = price;
            Guid = Guid.NewGuid();
        }
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        protected PriceTrend() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        public int Id { get; private set; }
        public Guid Guid { get; private set; }
        public int OfferId { get; private set; }         // FK value
        public Offer Offer { get; private set; }        // Navigation property
        public DateTime Date { get; private set; }
        public decimal Price { get; private set; }
    }
}
