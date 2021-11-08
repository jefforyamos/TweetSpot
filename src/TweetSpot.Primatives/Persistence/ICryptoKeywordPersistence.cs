using System.Threading.Tasks;

namespace TweetSpot.Persistence
{
    public interface ICryptoKeywordPersistence
    {
        string[] GetKeywords();
    }
}