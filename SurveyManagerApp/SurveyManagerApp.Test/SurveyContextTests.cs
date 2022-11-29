using SurveyManagerApp.Application.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace SurveyManagerApp.Test
{
    public class SurveyContextTests
    {
        [Fact]
        public void CreateDatabaseSuccessTest()
        {
            using var db = SurveyContext.ForTestConfig();
            db.Database.EnsureDeleted();
            db.Database.EnsureCreated();
            // If EnsureCreated does NOT throw an exception the test is successful.
            Assert.True(true);
        }
    }
}
