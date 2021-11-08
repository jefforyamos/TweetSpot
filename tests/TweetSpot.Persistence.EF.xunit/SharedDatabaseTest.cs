using System;
using TweetSpot.ServiceBus.Commands;
using Xunit;

namespace TweetSpot.Persistence.EF
{
    public class SharedDatabaseTest : IClassFixture<SharedDatabaseFixture>
    {
        public SharedDatabaseTest(SharedDatabaseFixture fixture) => Fixture = fixture;

        public SharedDatabaseFixture Fixture { get; }

        [Fact]
        public void Can_add_item()
        {
            using var transaction = Fixture.Connection.BeginTransaction();
            using var context = Fixture.CreateContext(transaction);
            var persistence = new TweetPersistence(context);
            var tweet1 = IncomingTweet.Create(@"{""data"":{""id"":""23430893958457012236"",""text"":""Inserted by unit test""}}", DateTime.UtcNow, 3);
            Assert.NotNull(tweet1);
            var primaryKey = persistence.Add(tweet1);
            var copy = persistence.Get(primaryKey);
            Assert.Equal(tweet1.UnparsedData, copy.UnparsedData);
        }
    }
}