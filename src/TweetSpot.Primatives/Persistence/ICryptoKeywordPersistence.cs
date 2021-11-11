using System.Threading.Tasks;

namespace TweetSpot.Persistence
{
    /// <summary>
    /// Defines the persistence needed to maintain keywords used to determine inclusion of tweets considered to be "about crypto".
    /// </summary>
    public interface ICryptoKeywordPersistence
    {
        /// <summary>
        /// Gets the list of currently declared keywords.
        /// </summary>
        /// <returns></returns>
        string[] GetKeywords();
    }
}