using System.ComponentModel.DataAnnotations;

namespace ExamManager.App.Entities
{
    /// <summary>
    /// Value object for address. Implemented as C#9 record
    /// (immutable). Needs to be configured with OwnsOne in
    /// ExamContext.OnModelCreating
    /// </summary>
    public record Address(
        [property: MaxLength(255)] string City,
        [property: MaxLength(255)] string Zip,
        [property: MaxLength(255)] string Street
        )
    {
        public string FullAddress => $"{Zip} {City}, {Street}";
    }
}
