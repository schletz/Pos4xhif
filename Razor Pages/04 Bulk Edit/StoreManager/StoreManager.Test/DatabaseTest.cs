using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using StoreManager.Application.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoreManager.Test
{
    public class DatabaseTest : IDisposable
    {
        private readonly SqliteConnection _connection;
        protected readonly StoreContext _db;

        public DatabaseTest()
        {
            _connection = new SqliteConnection("DataSource=:memory:");
            _connection.Open();
            var opt = new DbContextOptionsBuilder()
                .UseSqlite(_connection)  // Keep connection open (only needed with SQLite in memory db)
                .Options;

            _db = new StoreContext(opt);
        }
        public void Dispose()
        {
            _db.Dispose();
            _connection.Dispose();
        }
    }
}
