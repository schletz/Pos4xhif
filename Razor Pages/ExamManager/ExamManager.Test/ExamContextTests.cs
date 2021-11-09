using ExamManager.App.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace ExamManager.Test
{
    [Collection("Sequential")]
    public class ExamContextTests
    {
        private ExamContext GetContext()
        {
            var context = new ExamContext();
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();
            context.Seed();
            return context;
        }
        [Fact]
        public void EnsureDatabaseCreatedTest()
        {
            using var context = new ExamContext();
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();
            context.Seed();
        }

        [Fact]
        public void StudentsCountSuccessTest()
        {
            using (var context = new ExamContext())
            {
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();
                context.Seed();
            }
            using (var context = new ExamContext())
            {
                var schoolclass = context.SchoolClasses.First();
                Assert.True(schoolclass.StudentsCount > 0);
            }


        }
    }
}
