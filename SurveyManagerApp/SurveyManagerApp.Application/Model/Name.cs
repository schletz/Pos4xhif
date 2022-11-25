using System.ComponentModel.DataAnnotations;

namespace SurveyManagerApp.Application.Model
{
    /// <summary>
    /// We define value objects as records because value objects
    /// should be immutable.
    /// </summary>
    public record Name(
        [property:MaxLength(255)] string Firstname,
        [property:MaxLength(255)] string Lastname);
}
