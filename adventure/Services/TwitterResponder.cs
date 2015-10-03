using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LinqToTwitter;

namespace Adventure.Services
{
    public class TwitterResponder
    {
        public static async Task SendTweet(string status)
        {
            var twitterContext = new TwitterContext(TwitterHashtagMonitor.Authorizer);
            var tweet = await twitterContext.TweetAsync( status );
        }

        public static async Task SendTweetReply( string status, ulong replyId )
        {
            var twitterContext = new TwitterContext( TwitterHashtagMonitor.Authorizer );
            var tweet = await twitterContext.TweetAsync( status );
        }
    }
}
