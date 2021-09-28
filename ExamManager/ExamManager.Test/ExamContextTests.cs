using ExamManager.App.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace ExamManager.Test
{
    public class ExamContextTests
    {
        [Fact]
        public void EnsureDatabaseCreatedTest()
        {
            using var context = new ExamContext();
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();
            context.Seed();
        }
    }
}
