using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Adventure.Models;

namespace Adventure.Services
{
    public static class TweetParser
    {
        public static void Main( Tweet twitterMessage )
        {
            var twitterUser = twitterMessage.TweetId_num;
            var hashtags = twitterMessage.HashTags.ToList();
            if ( IsSubmitHashtagMissing( hashtags ) )
            {
                // Not a submission
                return;
            }

            int day = 0;
            foreach ( var dayString in hashtags.Select( hashtag => hashtag.Replace( "AdventHunt", "" ) ) )
            {
                if ( int.TryParse( dayString, out day ) )
                    break;
            }

            if ( day == 0 ) return;

            using ( var adventureContext = new AdventureContext() )
            {
                var challenge = GetChallengeForDay( adventureContext, day );
                if ( challenge == null )
                {
                    TwitterResponder.SendTweetReply( "Hey! That submission doesn't make any sense to us. Reply @adventiswhat if you think it should.", twitterUser );
                    return;
                }

                var user = GetUser( adventureContext, twitterMessage.TwitterUserIdentifier ) ?? NewUser( twitterMessage );

                var previouslyComplete = CheckChallengeComplete( adventureContext, challenge, user );
                if ( previouslyComplete == false )
                {
                    NewResponse( adventureContext, twitterMessage, challenge );
                    DetermineContent( twitterMessage, challenge, user, adventureContext );
                }
                else
                {
                    TwitterResponder.SendTweetReply( "Wow! You're enthusiastic! Looks like you've already completed that challenge.", twitterUser );

                    if ( twitterMessage.TimeStamp.Date > DateTime.Now.Date ) return;

                    var dayDifference = ( DateTime.Now.Date - twitterMessage.TimeStamp.Date ).Days;
                    TwitterResponder.SendTweetReply( "Wow, you're keen! You're a bit ahead of schedule with that #hashtag. Try again in " +
                        dayDifference + " days!", twitterUser );
                }
            }
            //If here is reached then they have not submitted a new challenge
        }

        private static bool CheckChallengeComplete( AdventureContext adventureContext, Challenge challenge, User user )
        {
            bool IsComplete = false;
            if ( adventureContext.UserChallenges
            .Where( u => u.ChallengeId == challenge.ChallengeId && u.UserId == user.UserId && u.IsComplete ).Count() >= 1 )
            {
                IsComplete = true;
            }
            return IsComplete;
        }

        private static User GetUser( AdventureContext adventureContext, string twitterUser )
        {
            var user = adventureContext
                .Users
                .FirstOrDefault( r => r.TwitterId == twitterUser );
            return user;
        }

        private static Challenge GetChallengeForDay( AdventureContext adventureContext, int day )
        {
            var challenge = adventureContext.Challenges
                .FirstOrDefault( c => c.ChallengeNumber == day && !c.Name.Contains( "Bonus" ) );
            return challenge;
        }

        public static bool IsSubmitHashtagMissing( List<string> hashtags )
        {
            return !hashtags.Any( h => h.ToLower() == "submit" );
        }

        private static User NewUser( Tweet twitterMessage )
        {
            var newUser = new User
            {
                TwitterId = twitterMessage.TwitterUserIdentifier,
                UserName = twitterMessage.UserName,
                ScreenName = twitterMessage.ScreenName
            };
            var adventureContext = new AdventureContext();
            adventureContext.Users.Add( newUser );
            adventureContext.SaveChanges();
            return newUser;
        }

        private static void NewResponse( AdventureContext adventureContext, Tweet tweet, Challenge challenge )
        {
            var user = adventureContext.Users
                .First( u => u.TwitterId == tweet.TwitterUserIdentifier );

            var response = new Response
            {
                UserId = user.UserId,
                Tweet = tweet.Text,
                TweetId = tweet.TweetId,
                ChallengeId = challenge.ChallengeId
            };
            adventureContext.Responses.Add( response );
            adventureContext.SaveChanges();

            VerifyBadges(adventureContext, user.UserId );
        }

        public static void DetermineContent( Tweet tweet, Challenge challenge, User user, AdventureContext adventureContext )
        {
            if (challenge == null) return;
            // Is image?
            if ( !( tweet.Media.FirstOrDefault() == null ) && challenge.Type.ToLower() == "image" )
            {
                SendResponse( tweet, challenge );
                CompleteChallenge( challenge, user, adventureContext );
            }
            // Is URL?
            else if ( tweet.Urls.Any() )
            {
                var youtubeTest = new Regex( "(https?://)?(www\\.)?(yotu\\.be/|youtube\\.com/)?((.+/)?(watch(\\?v=|.+&v=))?(v=)?)([\\w_-]{11})(&.+)?" );
                var instagramTest = new Regex( @"http://instagr\.?am(?:\.com)?/\S*" );
                var vineTest = new Regex( @"https://vine.co/v/\w*$@i" );
                var soundcloudTest = new Regex( @"(https?://)?(www\\.)?( soundcloud.com | snd.sc )(.)" );

                if ( youtubeTest.IsMatch( tweet.Urls.Any().ToString() ) && ( challenge.Type.ToLower() == "video" | challenge.Type.ToLower() == "audio" ) )
                {
                    SendResponse( tweet, challenge );
                    CompleteChallenge( challenge, user, adventureContext );

                }
                if ( instagramTest.IsMatch( tweet.Urls.Any().ToString() ) && challenge.Type.ToLower() == "image" )
                {
                    SendResponse( tweet, challenge );
                    CompleteChallenge( challenge, user, adventureContext );

                }
                if ( vineTest.IsMatch( tweet.Urls.Any().ToString() ) && challenge.Type.ToLower() == "video" )
                {
                    SendResponse( tweet, challenge );
                    CompleteChallenge( challenge, user, adventureContext );

                }

            }
            // Is text response
            else if (challenge.Type != null && challenge.Type.ToLower()=="Text")
            {
                string strippedTweet = StripContent( tweet );
                CompleteChallenge( challenge, user, adventureContext );
            }
            else
            {
                return;
            }
        }

        public static void CompleteChallenge( Challenge challenge, User user, AdventureContext adventureContext )
        {
            var userChallenge = new UserChallenge { ChallengeId = challenge.ChallengeId, UserId = user.UserId, IsComplete = true };
            adventureContext.UserChallenges.Add( userChallenge );
            adventureContext.SaveChanges();
        }

        private static string StripContent( Tweet tweet )
        {
            var removeMentions = new Regex( @"/(^|\b)@\S*($|\b)/" );
            var removeHashtags = new Regex( @"/(^|\b)#\S*($|\b)/" );
            var strippedTweet = removeMentions.Replace( tweet.Text, "" );
            strippedTweet = removeHashtags.Replace( strippedTweet, "" );
            return strippedTweet;
        }

        private static void SendResponse( Tweet tweet, Challenge challenge )
        {
            throw new NotImplementedException();
        }

        public static void TweetUser( string username, string message )
        {
            throw new NotImplementedException();
        }

        private static void VerifyBadges(AdventureContext context, int userId )
        {
            VerifyBadgeFirstParticipation(context, userId);
            VerifyBadgeFirstByType(context, userId, BadgeCodes.Audio);
            VerifyBadgeFirstByType(context, userId, BadgeCodes.Video);
            VerifyBadgeFirstByType(context, userId, BadgeCodes.Image);
            context.SaveChanges();
        }

        private static void VerifyBadgeFirstParticipation( AdventureContext context, int userId )
        {
            var badgeCount = ( from b in context.Badges
                               from ub in context.UserBadges.Where( x => x.UserId == userId )
                               where b.Code == BadgeCodes.FirstParticipation.ToString()
                               select b );

            if ( context.Responses.Count( x => x.UserId == userId ) == 1 && badgeCount.Count() == 0 )
            {
                var badge = context.Badges.First( x => x.Code == BadgeCodes.FirstParticipation.ToString() );

                if ( badge != null )
                {
                    context.UserBadges.Add( new UserBadge
                    {
                        BadgeId = badge.BadgeId,
                        UserId = userId
                    } );
                }
            }
        }

        private static void VerifyBadgeFirstByType( AdventureContext context, int userId, BadgeCodes code )
        {
            var result = ( from r in context.Responses
                           from c in context.Challenges.Where( x => x.ChallengeId == r.ChallengeId )
                           where r.UserId == userId
                           where c.Type == code.ToString()
                           select r );

            var badgeCount = ( from ub in context.UserBadges
                               from b in context.Badges.Where( x => x.BadgeId == ub.BadgeId )
                               where ub.UserId == userId
                               where b.Code == code.ToString()
                               select b );

            if ( result.Count() == 1 && badgeCount.Count() == 0 )
            {
                var badge = context.Badges.First( x => x.Code == code.ToString() );

                if ( badge != null )
                {
                    context.UserBadges.Add( new UserBadge
                    {
                        BadgeId = badge.BadgeId,
                        UserId = userId
                    } );
                }
            }
        }
    }
}