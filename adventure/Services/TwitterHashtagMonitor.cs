using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using LinqToTwitter;

namespace Adventure.Services
{
    public class TwitterHashtagMonitor
    {
        private static IAuthorizer _authorizer;
        private static IAuthorizer Authorizer
        {
            get
            {
                if (_authorizer != null) return _authorizer;

                var sessionStateCredentialStore = new SessionStateCredentialStore(HttpContext.Current.Session)
                {
                    ConsumerKey = Properties.Settings.Default.twitterConsumerKey,
                    ConsumerSecret = Properties.Settings.Default.twitterConsumerSecret,
                    OAuthToken = Properties.Settings.Default.twitterOAuthToken,
                    OAuthTokenSecret = Properties.Settings.Default.twitterAccessTokenSecret
                };
                _authorizer = new MvcAuthorizer
                {
                    CredentialStore = sessionStateCredentialStore
                };

                return _authorizer;
            }
        }

        public async Task Monitor()
        {
            var count = 0;
            var twitterCtx = new TwitterContext(Authorizer);
            await
                (from strm in twitterCtx.Streaming
                 where strm.Type == StreamingType.Filter &&
                       strm.Track == "twitter"
                 select strm)
                .StartAsync(async strm =>
                {
                    await Task.Run(() => Console.WriteLine(strm.Content + "\n"));

                    if (count++ >= 5)
                        strm.CloseStream();
                });
        }
    }
}