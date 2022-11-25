namespace SurveyManagerApp.Application.Model
{
    /// <summary>
    /// Use as type constraint in the generic repository.
    /// </summary>
    public interface IEntity<T>
    {
        T Id { get; }
    }
}
