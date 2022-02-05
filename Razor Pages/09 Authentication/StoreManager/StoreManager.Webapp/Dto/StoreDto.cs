using System;
using System.ComponentModel.DataAnnotations;

namespace StoreManager.Webapp.Dto
{
    class ValidCloseDate : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var store = validationContext.ObjectInstance as StoreDto;
            if (store is null) { return null; }

            if (store.CloseDate < DateTime.Now.Date.AddDays(-7))
                return new ValidationResult("Das Close Datum darf maximal 7 Tage zurück liegen.");

            if (store.CloseDate > DateTime.Now.Date.AddYears(1))
                return new ValidationResult("Das Close Datum darf maximal 1 Jahr in der Zukunft sein.");

            return ValidationResult.Success;
        }
    }
    public record StoreDto(
        Guid Guid,
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Der Name muss zwischen 2 und 100 Zeichen lang sein.")]
        string Name,
        [ValidCloseDate] DateTime? CloseDate,
        [Url(ErrorMessage = "Die URL ist nicht gültig")]
        string? Url);
}
