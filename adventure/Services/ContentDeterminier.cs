using System.Linq;
using System.Text.RegularExpressions;
using Adventure.Models;

namespace Adventure.Services
{
    static internal class ContentDeterminier
    {
        public static void DetermineContent(Tweet tweet, Challenge challenge, User user, AdventureContext adventureContext)
        {

            // Is image?
            if (tweet.Media.FirstOrDefault() != null && challenge.Type.ToLower() == "image")
            {
                TweetParser.CompleteChallenge(challenge, user, tweet, adventureContext);
            }
            // Is URL?
            else if (tweet.Urls.Any())
            {
                var youtubeTest = new Regex("(https?://)?(www\\.)?(yotu\\.be/|youtube\\.com/)?((.+/)?(watch(\\?v=|.+&v=))?(v=)?)([\\w_-]{11})(&.+)?");
                var instagramTest = new Regex(@"http://instagr\.?am(?:\.com)?/\S*");
                var vineTest = new Regex(@"https://vine.co/v/\w*$@i");
                var soundcloudTest = new Regex(@"(https?://)?(www\\.)?( soundcloud.com | snd.sc )(.)");

                if (youtubeTest.IsMatch(tweet.Urls.Any().ToString()) && (challenge.Type.ToLower() == "video" | challenge.Type.ToLower() == "audio"))
                {
                    TweetParser.CompleteChallenge(challenge, user, tweet, adventureContext);
                }
                if (instagramTest.IsMatch(tweet.Urls.Any().ToString()) && challenge.Type.ToLower() == "image")
                {
                    TweetParser.CompleteChallenge(challenge, user, tweet, adventureContext);

                }
                if (vineTest.IsMatch(tweet.Urls.Any().ToString()) && challenge.Type.ToLower() == "video")
                {
                    TweetParser.CompleteChallenge(challenge, user, tweet, adventureContext);
                }

            }
            // Is text response
            else if (challenge.Type != null && challenge.Type.ToLower() == "text")
            {
                tweet.Text = StripContent(tweet);

                TweetParser.CompleteChallenge(challenge, user, tweet, adventureContext);
            }
        }

        private static string StripContent(Tweet tweet)
        {
            var removeMentions = new Regex(@"/(^|\b)@\S*($|\b)/");
            var removeHashtags = new Regex(@"/(^|\b)#\S*($|\b)/");
            var strippedTweet = removeMentions.Replace(tweet.Text, "");
            strippedTweet = removeHashtags.Replace(strippedTweet, "");
            return strippedTweet;
        }

    }
}