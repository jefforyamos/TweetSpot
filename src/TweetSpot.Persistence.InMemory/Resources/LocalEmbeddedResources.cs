using TweetSpot.Resources;

namespace TweetSpot.Persistence.InMemory.Resources
{
    /// <summary>
    /// Provides access to embedded resources within this assembly.
    /// </summary>
    internal static class LocalEmbeddedResources
    {
        /// <summary>
        /// Contains the declaration of the keywords that are considered to be cryptocurrency-related.
        /// </summary>
        public static EmbeddedFileResource CryptoKeywords = new(typeof(LocalEmbeddedResources), "CryptoKeywords.txt");
    }
}
