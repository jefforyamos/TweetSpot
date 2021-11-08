using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TweetSpot.ServiceBus.Commands;

namespace TweetSpot.Persistence.EF
{
    public class TweetSpotDbContext : DbContext
    {
        public TweetSpotDbContext(DbContextOptions<TweetSpotDbContext> builder) : base(builder)
        {
            
        }
        public DbSet<IncomingTweet> Tweets { get; set; }

        public string DbPath { get; private set; }

        public TweetSpotDbContext()
        {
            var folder = Environment.SpecialFolder.LocalApplicationData;
            var path = Environment.GetFolderPath(folder);
            DbPath = $"{path}{System.IO.Path.DirectorySeparatorChar}tweets.db";
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite($"Data Source={DbPath}");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<IncomingTweet>(b =>
            {
                b.ToTable("Tweets");
                b.Property(nameof(IProcessIncomingTweet.Id));
                b.HasKey(nameof(IProcessIncomingTweet.Id));
                b.Property(e => e.ReceivedDateTimeUtc);
                b.Property(e => e.OrdinalCountNumber);
                b.Property(e => e.UnparsedData);
            });
        }
    }
}
