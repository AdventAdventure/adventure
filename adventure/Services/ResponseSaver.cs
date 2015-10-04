using System.Linq;
using Adventure.Models;

namespace Adventure.Services
{
    static internal class ResponseSaver
    {
        public static void SaveNewResponse(AdventureContext adventureContext, Tweet tweet, Challenge challenge)
        {
            var user = adventureContext.Users
                .First(u => u.TwitterId == tweet.TwitterUserIdentifier);

            var response = new Response
                           {
                               UserId = user.UserId,
                               Tweet = tweet.Text,
                               TweetId = tweet.TweetId,
                               ChallengeId = challenge.ChallengeId
                           };
            adventureContext.Responses.Add(response);
            adventureContext.SaveChanges();
        }
    }
}