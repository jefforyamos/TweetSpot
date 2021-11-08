using TweetSpot.ServiceBus.Commands;

namespace TweetSpot.Persistence.EF
{
    public class TweetPersistence : ITweetPersistence 
    {
        private readonly TweetSpotDbContext _context;

        public TweetPersistence(TweetSpotDbContext context)
        {
            _context = context;
        }
        public ulong Add(IncomingTweet tweet)
        {
            var result = _context.Tweets.Add(tweet);
            return result.Entity.Id;
        }

        public IncomingTweet Get(ulong id)
        {
            var result = _context.Tweets.Find(id);
            return result;
        }
    }
}