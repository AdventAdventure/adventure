using System;
using System.Collections.Generic;
using System.Linq;
using Adventure.Models;

namespace Adventure.Services
{
    public static class TweetParser
    {
        public static void Main(Tweet twitterMessage)
        {
            var tweetId = twitterMessage.TweetId_num;
            var hashtags = twitterMessage.HashTags.Select(h => h.ToLower()).ToList();
            if (IsSubmitHashtagMissing(hashtags))
            {
                // Not a submission
                return;
            }

            var day = FindDayFromHashtag(hashtags);

            if (day == 0) return;

            using (var adventureContext = new AdventureContext())
            {
                var challenge = GetChallengeForDay(adventureContext, day);
                if (challenge == null)
                {
                    TwitterResponder.SendTweetReply(twitterMessage.ScreenName, "Hey! That submission doesn't make any sense to us. Reply @adventiswhat if you think it should.", tweetId);
                    return;
                }

                var user = GetUser(adventureContext, twitterMessage.TwitterUserIdentifier) ?? NewUser(twitterMessage);

                var previouslyComplete = CheckChallengeComplete(adventureContext, challenge, user);
                if (previouslyComplete == false)
                {
                    ResponseSaver.SaveNewResponse(adventureContext, twitterMessage, challenge);
                    ContentDeterminier.DetermineContent(twitterMessage, challenge, user, adventureContext);
                    return;
                }
                ReplyWithError(twitterMessage, tweetId);
            }
        }

        private static void ReplyWithError(Tweet twitterMessage, ulong twitterUser)
        {
            var message = twitterMessage.TimeStamp.Date > DateTime.Now.Date
                ? ReplyWithAlreadyAnswered(twitterMessage)
                : ReplyWithTooSoon();
            TwitterResponder.SendTweetReply( twitterMessage.ScreenName, message, twitterUser);
        }

        private static string ReplyWithTooSoon()
        {
            var message = "Wow! You're enthusiastic! Looks like you've already completed that challenge.";
            return message;
        }

        private static string ReplyWithAlreadyAnswered(Tweet twitterMessage)
        {
            var dayDifference = (DateTime.Now.Date - twitterMessage.TimeStamp.Date).Days;
            return  "Wow, you're keen! You're a bit ahead of schedule with that #hashtag. Try again in " +
                            dayDifference + " days!";
        }

        private static int FindDayFromHashtag(IEnumerable<string> hashtags)
        {
            var day = 0;
            foreach (var dayString in hashtags.Select(hashtag => hashtag.Replace("AdventHunt".ToLower(), "")))
            {
                if (int.TryParse(dayString, out day))
                    break;
            }
            return day;
        }

        private static bool CheckChallengeComplete(AdventureContext adventureContext, Challenge challenge, User user)
        {
            return adventureContext
                .UserChallenges
                .Count(u =>
                    u.ChallengeId == challenge.ChallengeId &&
                    u.UserId == user.UserId &&
                    u.IsComplete) >= 1;
        }

        private static User GetUser(AdventureContext adventureContext, string twitterUser)
        {
            var user = adventureContext
                .Users
                .FirstOrDefault(r => r.TwitterId == twitterUser);
            return user;
        }

        private static Challenge GetChallengeForDay(AdventureContext adventureContext, int day)
        {
            var challenge = adventureContext.Challenges
                .FirstOrDefault(c => c.ChallengeNumber == day && !c.Name.Contains("Bonus"));
            return challenge;
        }

        public static bool IsSubmitHashtagMissing(List<string> hashtags)
        {
            return hashtags.All(h => h.ToLower() != "submit");
        }

        private static User NewUser(Tweet twitterMessage)
        {
            var newUser = new User
            {
                TwitterId = twitterMessage.TwitterUserIdentifier,
                UserName = twitterMessage.UserName,
                ScreenName = twitterMessage.ScreenName
            };
            var adventureContext = new AdventureContext();
            adventureContext.Users.Add(newUser);
            adventureContext.SaveChanges();
            return newUser;
        }


        public static void CompleteChallenge(Challenge challenge, User user, Tweet tweet, AdventureContext adventureContext)
        {
            var userChallenge = new UserChallenge
            {
                ChallengeId = challenge.ChallengeId,
                UserId = user.UserId,
                IsComplete = true
            };
            adventureContext.UserChallenges.Add(userChallenge);
            adventureContext.SaveChanges();
            TwitterResponder.SendMoreInfo( user.ScreenName, challenge, tweet.TweetId_num);
            new BadgeFinder(adventureContext).VerifyBadges(user.UserId);
        }
    }
}