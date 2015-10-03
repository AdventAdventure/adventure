using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LinqToTwitter;

namespace Adventure.Services
{
    public class TwitterHashtagMonitor
    {
        private static IAuthorizer _authorizer;
        private static IAuthorizer Authorizer => _authorizer ?? (_authorizer = new MvcAuthorizer
        {
            CredentialStore = new InMemoryCredentialStore
            {
                ConsumerKey = Properties.Settings.Default.twitterConsumerKey,
                ConsumerSecret = Properties.Settings.Default.twitterConsumerSecret,
                OAuthToken = Properties.Settings.Default.twitterOAuthToken,
                OAuthTokenSecret = Properties.Settings.Default.twitterAccessTokenSecret
            }
        });

        public async Task Monitor()
        {
            var search = // "AFLGF," + 
                string.Join(",", Enumerable.Range(1, 24).Select(d => "AdventHunt" + d));

            var twitterCtx = new TwitterContext(Authorizer);
            await
                (from strm in twitterCtx.Streaming
                 where strm.Type == StreamingType.Filter &&
                       strm.Track == search
                 select strm)
                .StartAsync(async streamContent =>
                {
                    await HandleTweetArrival(streamContent);
                });
        }

        private static async Task HandleTweetArrival(StreamContent streamContent)
        {
            var status = streamContent?.Entity as Status;

            if (status == null)
                return;

            var tweet = new Tweet
            {
                TwitterUserIdentifier = status.User.UserIDResponse,
                ScreenName = status.User.ScreenNameResponse,
                UserName = status.User.Name,
                HashTags = status.Entities.HashTagEntities.Select(h => h.Tag),
                Text = status.Text,
                TimeStamp = status.CreatedAt
            };
            await Task.Yield();
        }
    }

    public class Tweet
    {
        public string TwitterUserIdentifier { get; set; }
        public string ScreenName { get; set; }
        public string UserName { get; set; }
        public IEnumerable<string> HashTags { get; set; }
        public string Text { get; set; }
        public DateTime TimeStamp { get; set; }
    }
}