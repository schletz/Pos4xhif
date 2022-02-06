using System;
using System.ComponentModel.DataAnnotations;

namespace StoreManager.Webapp.Dto
{
    public record OfferDto(
        Guid Guid,
        [Range(0, 1000000, ErrorMessage = "Ungültiger Preis")]
        decimal Price,
        bool SoldOut,
        Guid ProductGuid,
        Guid StoreGuid);
}
