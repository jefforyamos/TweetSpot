namespace TweetSpot.Models
{
    public static class TwitterFeedConfigurationExtensions
    {
        /// <summary>
        /// Returns a displayable version of the bearer token to confirm the value being used without disclosing the value in it's entirety.
        /// </summary>
        public static string GetTwitterBearerTokenAbbreviation(this ITwitterFeedConfiguration configuration)
        {
            const int minimumLength = 10;
            var token = configuration.TwitterBearerToken;
            var length = token?.Length ?? 0;
            if (length < minimumLength) return "<BAD TOKEN>";
            var beginSegment = token?.Substring(0, 3) ?? string.Empty;
            var endSegment = token?.Substring(length - 3, 3) ?? string.Empty;
            return $"{beginSegment}...{endSegment}";
        }

        ///// <summary>
        ///// Examines the configuration throwing a fatal exception identifying missing settings.  To be called at startup.
        ///// </summary>
        ///// <exception cref="EnvironmentConfigurationException">Identifies the name of the first setting that was missing from the environment.</exception>
        public static void DemandEssentialSettings(this ITwitterFeedConfiguration configuration)
        {

        }

    }
}
