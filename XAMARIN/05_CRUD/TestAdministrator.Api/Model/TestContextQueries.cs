using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TestAdministrator.Api.Model
{
    /// <summary>
    /// Klasse für alle benutzerdefinierten Abfragen, die häufiger verwendet werden.
    /// </summary>
    public partial class TestsContext
    {
        /// <summary>
        /// Liefert alle Unterrichtenden Lehrer lt. Stundenplan einer Klasse.
        /// </summary>
        /// <param name="schoolClass">Klasse, dessen Lehrer abgefragt werden.</param>
        /// <returns></returns>
        public IQueryable<Teacher> GetClassTeachers(string schoolClass)
        {
            if (string.IsNullOrEmpty(schoolClass))
            { return Enumerable.Empty<Teacher>().AsQueryable(); }
            // Achtung: Vergleiche in SQLite sind Case Sensitive, daher ToUpper().
            var teacherIds = Lesson
                .Where(l => l.L_Class.ToUpper() == schoolClass.ToUpper())
                .Select(l => l.L_Teacher);
            return Teacher.Where(t => teacherIds.Contains(t.T_ID));
        }
    }
}
