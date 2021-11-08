using System;
using System.Data.Common;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace TweetSpot.Persistence.EF
{
    public class TweetPersistenceTestSqlInMemory : TweetPersistenceTest, IDisposable
    {
        private readonly DbConnection _connection;

        public TweetPersistenceTestSqlInMemory()
            : base(
                new DbContextOptionsBuilder<TweetSpotDbContext>()
                    .UseSqlite(CreateInMemoryDatabase())
                    .Options)
        {
            _connection = RelationalOptionsExtension.Extract(ContextOptions).Connection;
        }

        private static DbConnection CreateInMemoryDatabase()
        {
            var connection = new SqliteConnection("Filename=:memory:");

            connection.Open();

            return connection;
        }

        public void Dispose() => _connection.Dispose();
    }
}