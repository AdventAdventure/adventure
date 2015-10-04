using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LinqToTwitter;
using Adventure.Models;

namespace Adventure.Services
{
    public class TwitterResponder
    {
        public static void SendTweet(string twitterUser, string status)
        {
            var twitterContext = new TwitterContext(TwitterHashtagMonitor.Authorizer);
            Task.FromResult(twitterContext.TweetAsync( status ));
        }

        public static void SendTweetReply( string twitterUser, string status, ulong replyId )
        {
            var twitterContext = new TwitterContext( TwitterHashtagMonitor.Authorizer );
            var tweet = string.Format( "@{0} {1}", twitterUser, status);
            Task.FromResult(twitterContext.ReplyAsync( replyId, tweet ) );
        }

        public static void SendMoreInfo( string twitterUser, Challenge challenge, ulong replyId )
        {
            var twitterContext = new TwitterContext( TwitterHashtagMonitor.Authorizer );
            var tweet = string.Format( "@{0} Congratulations! You earned {1} points, find out more about today's challenge at http://adventure-1.apphb.com/#/day/{2}/more", twitterUser, challenge.Value, challenge.ChallengeNumber );
            Task.FromResult( twitterContext.ReplyAsync( replyId, tweet ) );
        }

    }
}
