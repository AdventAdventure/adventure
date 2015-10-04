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
        public static void SendTweet(string status)
        {
            var twitterContext = new TwitterContext(TwitterHashtagMonitor.Authorizer);
            Task.FromResult(twitterContext.TweetAsync( status ));
        }

        public static void SendTweetReply( string status, ulong replyId )
        {
            var twitterContext = new TwitterContext( TwitterHashtagMonitor.Authorizer );
            Task.FromResult(twitterContext.TweetAsync( status ));
        }
    }
}
