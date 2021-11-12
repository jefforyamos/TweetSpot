using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TweetSpot.Resources
{
    public static class GlobalTestResources
    {
        public static TwitterFeedEmbeddedFileResource TweetStreamSmall1 = new TwitterFeedEmbeddedFileResource(typeof(GlobalTestResources), "TweetStreamSmall_1.txt");
    }

 
}
