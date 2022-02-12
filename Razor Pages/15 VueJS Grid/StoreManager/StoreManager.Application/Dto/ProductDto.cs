using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoreManager.Application.Dto
{
    public record ProductDto(
        Guid Guid,
        [RegularExpression("^[1-9][0-9]{5}$", ErrorMessage = "Die EAN Nummer muss 6 Stellen haben.")]
        int Ean,
        [StringLength(255, MinimumLength = 2, ErrorMessage = "Der Produktname muss zwischen 2 und 255 Stellen lang sein.")]
        string Name,
        Guid ProductCategoryGuid,
        [Range(0, 999999, ErrorMessage = "Der Preis darf max. 999.999 Euro betragen.")]
        decimal? RecommendedPrice = null,
        [Range(typeof(DateTime), "2020-01-01", "2100-01-01", ErrorMessage = "Ungültiges Datum.")]
        DateTime? AvailableFrom = null);
}
