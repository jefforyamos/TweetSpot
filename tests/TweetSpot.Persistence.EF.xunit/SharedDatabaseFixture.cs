using System;
using System.Data.Common;
using System.Data.SqlClient;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using TweetSpot.ServiceBus.Commands;

namespace TweetSpot.Persistence.EF
{
    /// <summary>
    /// Sharable fixture for use in testing.
    /// </summary>
    /// <remarks>
    /// https://docs.microsoft.com/en-us/ef/core/testing/sharing-databases
    /// </remarks>
    public class SharedDatabaseFixture : IDisposable
    {
        private static readonly object _lock = new object();
        private static bool _databaseInitialized;

        public SharedDatabaseFixture()
        {
            Connection = CreateInMemoryDatabase();
            Seed();
            Connection.Open();
        }

        public DbConnection Connection { get; private set; }

        public TweetSpotDbContext CreateContext(DbTransaction transaction = null)
        {
            var context = new TweetSpotDbContext(new DbContextOptionsBuilder<TweetSpotDbContext>()
                .UseSqlite(Connection)
                .EnableSensitiveDataLogging()
                .Options);

            if (transaction != null)
            {
                context.Database.UseTransaction(transaction);
            }

            return context;
        }

        private static DbConnection CreateInMemoryDatabase()
        {
            var connection = new SqliteConnection("Filename=:memory:");

            // connection.Open();

            return connection;
        }

        private void Seed()
        {
            lock (_lock)
            {
                if (!_databaseInitialized)
                {
                    using (var context = CreateContext())
                    {
                        context.Database.EnsureDeleted();
                        context.Database.EnsureCreated();
                        context.Database.Migrate();

                        var one = IncomingTweet.Create(@"{""data"":{""id"":""1230893958457012235"",""text"":""Hey, this is my first tweet!""}}", DateTime.UtcNow, 1);
                        var two = IncomingTweet.Create(@"{""data"":{""id"":""1230893958457012236"",""text"":""Hey, this is my second tweet!""}}", DateTime.UtcNow, 2);

                        context.Tweets.AddRange(one, two);

                        context.SaveChanges();
                    }

                    _databaseInitialized = true;
                }
            }
        }

        public void Dispose() => Connection.Dispose();
    }
}