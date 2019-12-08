using SPG.CodeFirstApplication.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SPG.CodeFirstApplication
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CrudMethods crudMethods = new CrudMethods();

            Console.WriteLine("EF-Core Code First Beispiel");
            Console.WriteLine("");

            Console.WriteLine("Liste aller Klassen (vor Add):");
            crudMethods.ListSchoolClasses();

            Console.WriteLine("2 Schüler und eine Klasse hinzufügen:");
            Console.WriteLine("");
            crudMethods.AddSchoolClass();

            Console.WriteLine("Liste aller Klassen (nach Add):");
            Console.WriteLine("");
            crudMethods.ListSchoolClasses();

            Console.WriteLine("Update 5AHIF:");
            Console.WriteLine("");
            crudMethods.UpdateSchoolClass();

            Console.WriteLine("Liste aller Klassen (nach Update):");
            Console.WriteLine("");
            crudMethods.ListSchoolClasses();
        }
    }

    public class CrudMethods
    {
        public void ListSchoolClasses()
        {
            //using (var context = new SchoolContextFactory().CreateDbContext())
            //{
            //    List<SchoolClass> schoolClasses = context.SchoolClasses.ToList();
            //    foreach (SchoolClass item in schoolClasses)
            //    {
            //        Console.WriteLine($"{item.Name}\t{item.Department}\t{item.Id}");
            //    }
            //}
        }

        public void AddSchoolClass()
        {
            SchoolClass newSchoolClass01 = new SchoolClass() { Id = new Guid("1e75cf1c-1085-4284-a7fa-0600619c8541"), Name = "5AHIF", Department = "Höhere Informatik" };
            Pupil newPupil01 = new Pupil() { Id = new Guid("87e46d36-74e1-4728-bc13-bb96436b3c83"), FirstName = "Vorname 16", LastName = "Nachname 16", Gender = "W" };
            Pupil newPupil02 = new Pupil() { Id = new Guid("8a00daf3-cfff-4f49-98bb-7025a8536797"), FirstName = "Vorname 17", LastName = "Nachname 17", Gender = "W" };

            //using ...
        }

        public void UpdateSchoolClass()
        {

            //using ...
        }
    }
}
