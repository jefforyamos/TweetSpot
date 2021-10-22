namespace TweetSpot.ServiceBus.Events
{
    public interface ITwitterFeedInitStarted
    {
        string BearerTokenAbbreviation { get; }
    }
}