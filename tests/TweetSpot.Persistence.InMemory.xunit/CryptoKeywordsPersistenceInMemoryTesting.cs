using Xunit;

namespace TweetSpot.Persistence.InMemory
{
    public class CryptoKeywordsPersistenceInMemoryTesting
    {
        [Fact]
        public void GetKeywords_Normal_Succeeds()
        {
            var svc = new CryptoKeywordPersistenceInMemory();
            var keywords = svc.GetKeywords();
            Assert.Contains("$BTC", keywords);
        }

        [Theory]
        [InlineData("#BTC")]
        [InlineData("#ETH")]
        [InlineData("Bitcoin")]
        public void GetKeywords_Normal_ContainsExpectedExpression(string expectedValue)
        {
            var svc = new CryptoKeywordPersistenceInMemory();
            var keywords = svc.GetKeywords();
            Assert.Contains(expectedValue, keywords);
        }
    }
}