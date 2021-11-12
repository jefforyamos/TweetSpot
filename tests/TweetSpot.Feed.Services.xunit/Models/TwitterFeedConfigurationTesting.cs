using Microsoft.Extensions.Configuration;
using Moq;
using Xunit;
using Xunit.Abstractions;


namespace TweetSpot.Models
{
    public class TwitterFeedConfigurationTesting
    {
        private readonly ITestOutputHelper _output;

        public TwitterFeedConfigurationTesting(ITestOutputHelper output)
        {
            _output = output;
        }


        [Theory(DisplayName = "The abbreviation for the bearer token should include digits from begin and end with elipisis in between")]
        [InlineData("01234567890ABCDEFGHIJKLMNOPQRSTUVWXYZ", "012...XYZ")]
        [InlineData("01234567890ABCDEFGHIJKLMNOPQRSTUVWXYZ987654321", "012...321")]
        public void TwitterBearerTokenAbbreviation_Normal_AsExpected(string bearerTokenValue, string expectedResult)
        {
            var config = new Mock<ITwitterFeedConfiguration>();
            config.Setup(c => c.TwitterBearerToken).Returns(bearerTokenValue);
            config.Setup(c => c.TwitterBearerTokenAbbreviation).CallBase();
            var result = config.Object.TwitterBearerTokenAbbreviation;
            Assert.Equal(expectedResult, result);
        }

        [Fact]
        public void TwitterBearerToken_EnvOverride_ShouldMatchEnv()
        {
            const string envValue = "823230492304830";
            var envConfig = new Mock<IConfiguration>();
            envConfig.Setup(e => e[TwitterFeedConfiguration.TwitterBearerTokenKey]).Returns(envValue);
            var config = new TwitterFeedConfiguration(envConfig.Object);
            Assert.Equal(envValue, config.TwitterBearerToken);
        }

        [Fact]
        public void ClientTimeout_EnvIsDefaulted_ShouldMatchDefaultValue()
        {
            var envConfig = new Mock<IConfiguration>();
            var config = new TwitterFeedConfiguration(envConfig.Object);
            Assert.Equal(TwitterFeedConfiguration.ClientTimeoutDefault, config.ClientTimeout);
        }

        [Theory]
        [InlineData("00:00:05", 5.00)] // Should parse as a valid TimeSpan
        [InlineData("5", 5.00)] // Should be understood as the number of seconds
        public void ClientTimeout_EnvOverride_ShouldMatchEnv(string envSpecifiedValue, double totalTimeInSeconds)
        {
            var envConfig = new Mock<IConfiguration>();
            envConfig.Setup(e => e[TwitterFeedConfiguration.TwitterClientTimeoutKey]).Returns(envSpecifiedValue);
            var config = new TwitterFeedConfiguration(envConfig.Object);
            _output.WriteLine($"Specifying [{envSpecifiedValue}] in the environment is interpreted as {config.ClientTimeout}, or {config.ClientTimeout.TotalSeconds} seconds.");
            Assert.Equal(totalTimeInSeconds, config.ClientTimeout.TotalSeconds);
        }
    }
}