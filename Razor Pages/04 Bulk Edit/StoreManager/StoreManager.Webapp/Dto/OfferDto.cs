using System;
using System.ComponentModel.DataAnnotations;

namespace StoreManager.Webapp.Dto
{
    public record OfferDtoBase(
        Guid Guid,
        [Range(0, 1000000, ErrorMessage ="Ungültiger Preis")]
        decimal Price,
        bool SoldOut);

    public record OfferDto(
        Guid Guid,
        decimal Price,
        bool SoldOut,
        DateTime LastUpdate,
        int ProductEan,
        int StoreId) : OfferDtoBase(Guid, Price, SoldOut);
}
