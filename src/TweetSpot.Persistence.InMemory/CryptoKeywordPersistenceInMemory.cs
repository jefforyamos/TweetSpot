using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TweetSpot.Persistence.InMemory.Resources;

namespace TweetSpot.Persistence.InMemory
{
    public class CryptoKeywordPersistenceInMemory : ICryptoKeywordPersistence
    {
        private string[]? _cache;
        public string[] GetKeywords()
        {
            return _cache ?? ( _cache = GetKeywordsFromResourceFile().Where( w => !string.IsNullOrWhiteSpace(w)).ToArray() );
        }

        private IEnumerable<string> GetKeywordsFromResourceFile()
        {
            using var stream = LocalEmbeddedResources.CryptoKeywords.OpenReadStream();
            if (stream == null) throw new InvalidOperationException("Embedded resource not found.");
            using var streamReader = new StreamReader(stream);
            string? line = null;
            while ((line = streamReader.ReadLine()) != null) yield return line;
        }

    }
}