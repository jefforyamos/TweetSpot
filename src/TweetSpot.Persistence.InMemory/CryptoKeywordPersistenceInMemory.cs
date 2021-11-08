using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace TweetSpot.Persistence.InMemory
{
    public class CryptoKeywordPersistenceInMemory : ICryptoKeywordPersistence
    {
        public string[] GetKeywords()
        {
            return GetKeywordsFromResourceFile().Where( w => !string.IsNullOrWhiteSpace(w)).ToArray();
        }

        private IEnumerable<string> GetKeywordsFromResourceFile()
        {
            using var memoryStream = new MemoryStream(Properties.Resources.CryptoKeywords);
            using var streamReader = new StreamReader(memoryStream);
            string? line = null;
            while ((line = streamReader.ReadLine()) != null) yield return line;
        }
    }
}