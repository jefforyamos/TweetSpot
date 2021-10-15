using System.Threading.Tasks;

namespace JeffTwit.Components
{
    public interface ITwitterFeedConsumer
    {
        Task ConsumeAsync(ITwitterFeedProvider provider);
    }
}