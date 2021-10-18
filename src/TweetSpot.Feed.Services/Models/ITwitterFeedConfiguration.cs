using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TweetSpot.Models
{
    public interface ITwitterFeedConfiguration
    {
        string TwitterBearerToken { get; }
    }
}
