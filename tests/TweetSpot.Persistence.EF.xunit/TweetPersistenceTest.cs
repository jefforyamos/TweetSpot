using System.Linq;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace TweetSpot.Persistence.EF
{
    public abstract class TweetPersistenceTest
    {
        protected TweetPersistenceTest(DbContextOptions<TweetSpotDbContext> contextOptions)
        {
            ContextOptions = contextOptions;

            Seed();
        }

        protected DbContextOptions<TweetSpotDbContext> ContextOptions { get; }

        private void Seed()
        {
            //using (var context = new ItemsContext(ContextOptions))
            //{
            //    context.Database.EnsureDeleted();
            //    context.Database.EnsureCreated();

            //    var one = new Item("ItemOne");
            //    one.AddTag("Tag11");
            //    one.AddTag("Tag12");
            //    one.AddTag("Tag13");

            //    var two = new Item("ItemTwo");

            //    var three = new Item("ItemThree");
            //    three.AddTag("Tag31");
            //    three.AddTag("Tag31");
            //    three.AddTag("Tag31");
            //    three.AddTag("Tag32");
            //    three.AddTag("Tag32");

            //    context.AddRange(one, two, three);

            //    context.SaveChanges();
            //}
        }

        [Fact]
        public void Test1()
        {
            using var context = new TweetSpotDbContext(ContextOptions);
            var test = context.Tweets.ToArray();
            Assert.Single(test);
        }
    }
}