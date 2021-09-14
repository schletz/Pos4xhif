using System.ComponentModel.DataAnnotations;

namespace ExamManager.App.Entities
{
    // Positional record
    public record Address(
        [property: MaxLength(255)] string City,
        [property: MaxLength(255)] string Zip,
        [property: MaxLength(255)] string Street
        )
    {
        public string FullAddress => $"{Zip} {City}, {Street}";
    }
}
