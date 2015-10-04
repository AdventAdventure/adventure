using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Adventure.Models;

namespace Adventure.Services
{
    public static class TweetParser
    {
        public static void Main(Tweet twitterMessage)
        {
            var twitterUser = twitterMessage.TweetId_num;
            var hashtags = twitterMessage.HashTags.ToList();
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
                    TwitterResponder.SendTweetReply("Hey! That submission doesn't make any sense to us. Reply @adventiswhat if you think it should.", twitterUser);
                    return;
                }

                var user = GetUser(adventureContext, twitterMessage.TweetId) ?? NewUser(twitterMessage);

                var previouslyComplete = CheckChallengeComplete(adventureContext, challenge, user);
                if (previouslyComplete == false)
                {
                    NewResponse(adventureContext, twitterMessage, challenge);
                    DetermineContent(twitterMessage, challenge, user, adventureContext);
                    return;
                }
                if (twitterMessage.TimeStamp.Date > DateTime.Now.Date)
                {
                    ReplyWithAlreadyAnswered(twitterMessage, twitterUser);
                }
                else
                {
                    ReplyWithTooSoon(twitterUser);
                }
            }
        }

        private static void ReplyWithTooSoon(ulong twitterUser)
        {
            TwitterResponder.SendTweetReply(
                "Wow! You're enthusiastic! Looks like you've already completed that challenge.", twitterUser);
        }

        private static void ReplyWithAlreadyAnswered(Tweet twitterMessage, ulong twitterUser)
        {
            var dayDifference = (DateTime.Now.Date - twitterMessage.TimeStamp.Date).Days;
            TwitterResponder.SendTweetReply(
                "Wow, you're keen! You're a bit ahead of schedule with that #hashtag. Try again in " +
                dayDifference + " days!", twitterUser);
        }

        private static int FindDayFromHashtag(List<string> hashtags)
        {
            int day = 0;
            foreach (var dayString in hashtags.Select(hashtag => hashtag.Replace("AdventHunt", "")))
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

        private static void NewResponse(AdventureContext adventureContext, Tweet tweet, Challenge challenge)
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


        public static void DetermineContent(Tweet tweet, Challenge challenge, User user, AdventureContext adventureContext)
        {
            // Is image?
            if (tweet.Media.FirstOrDefault() != null && challenge.Type.ToLower() == "image")
            {
                CompleteChallenge(challenge, user, tweet, adventureContext);
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
                    CompleteChallenge(challenge, user, tweet, adventureContext);
                }
                if (instagramTest.IsMatch(tweet.Urls.Any().ToString()) && challenge.Type.ToLower() == "image")
                {
                    CompleteChallenge(challenge, user, tweet, adventureContext);

                }
                if (vineTest.IsMatch(tweet.Urls.Any().ToString()) && challenge.Type.ToLower() == "video")
                {
                    CompleteChallenge(challenge, user, tweet, adventureContext);
                }

            }
            // Is text response
            else if (challenge.Type.ToLower() == "Text")
            {
                tweet.Text = StripContent(tweet);

                CompleteChallenge(challenge, user, tweet, adventureContext);
            }
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
            TwitterResponder.SendTweetReply(challenge.InfoResponse, tweet.TweetId_num);
            new BadgeFinder(adventureContext).VerifyBadges(user.UserId);
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