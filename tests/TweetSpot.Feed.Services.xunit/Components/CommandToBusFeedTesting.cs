using System.Threading.Tasks;
using Xunit;

namespace TweetSpot.Components
{
    public class CommandToBusFeedTesting
    {
        [Fact]
        public async Task SubmittedToBus_SingleMessage_SubmittedIntact()
        {
            await Task.CompletedTask;
        }
    }
}
