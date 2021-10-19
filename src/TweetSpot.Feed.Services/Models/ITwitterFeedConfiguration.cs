using System;

namespace TweetSpot.Models
{
    public interface ITwitterFeedConfiguration
    {
        string TwitterBearerToken { get; }

        Uri SampledStreamUri { get; }
    }
}
