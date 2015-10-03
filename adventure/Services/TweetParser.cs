using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text.RegularExpressions;
using Adventure.Models;

namespace Adventure.Services
{
    public static class TweetParser
    {
        public static void main( Tweet TwitterMessage )
        {
            var twitterUser = TwitterMessage.TwitterUserIdentifier;
            var hashtags = TwitterMessage.HashTags;
            if ( hashtags.Any( h => !h.Contains( "submit" ) ) )
            {
                // Not a submission
                return; /////<---- FIX
            }
            List<int> submitted = new List<int>();
            foreach ( var hashtag in hashtags )
            {
                int day = int.Parse( Regex.Split( hashtag, "(AdventHunt)((?:[0-9]+))" )[1] );
                var adventureContext = new AdventureContext();
                var challenge = adventureContext.Challenges
                    .Where( c => c.ChallengeNumber == day )
                    .FirstOrDefault();
                if ( challenge == null )
                {
                    TweetUnknown( twitterUser );
                    break;
                }
                var user = adventureContext.Users
                    .Where( r => r.TwitterId == twitterUser )
                    .FirstOrDefault();
                if ( user == null )
                {
                    user = NewUser( TwitterMessage );
                }
                var response = adventureContext.Responses
                    .Where( r => r.ChallengeId == challenge.ChallengeId & r.UserId == user.UserId )
                    .FirstOrDefault();
                if ( response == null )
                {
                    NewResponse( TwitterMessage, challenge );
                    DetermineContent( TwitterMessage, challenge );
                }
                else
                {
                    if ( response != null )
                    {
                        TweetUser( twitterUser, "Wow! You're enthusiastic! Looks like you've already completed that challenge." );
                    }

                    if ( TwitterMessage.TimeStamp.Date <= DateTime.Now.Date )
                    {
                        int dayDifference = ( DateTime.Now.Date - TwitterMessage.TimeStamp.Date ).Days;
                        TweetUser( twitterUser, "Wow, you're keen! You're a bit ahead of schedule with that #hashtag. Try again in " + dayDifference + " days!" );
                    }
                }
            }
            //If here is reached then they have not submitted a new challenge
        }

        public static Models.User NewUser( Tweet TwitterMessage )
        {
            var newUser = new Models.User
            {
                TwitterId = TwitterMessage.TwitterUserIdentifier,
                UserName = TwitterMessage.UserName
            };
            var adventureContext = new AdventureContext();
            adventureContext.Users.Add( newUser );
            return newUser;
        }

        public static Models.Response NewResponse( Tweet tweet, Models.Challenge challenge )
        {
            var user = new AdventureContext().Users
                .Where( u => u.TwitterId == tweet.TwitterUserIdentifier )
                .FirstOrDefault();
            var response = new Models.Response
            {
                UserId = user.UserId,
                Tweet = tweet.Text,
                TweetId = tweet.TweetId,
                ChallengeId = challenge.ChallengeId
            };
            return response;
        }

        public static void TweetUnknown( string twitterUser )
        {
            // Change our account!!!
            TweetUser( twitterUser, "Hey! That submission doesn't make any sense to us. Reply @adventiswhat if you think it should." );
        }

        public static string StripContent( Tweet tweet )
        {
            Regex removeMentions = new Regex( @"/(^|\b)@\S*($|\b)/" );
            Regex removeHashtags = new Regex( @"/(^|\b)#\S*($|\b)/" );
            string strippedTweet = removeMentions.Replace( tweet.Text, "" );
            strippedTweet = removeHashtags.Replace( strippedTweet, "" );
            return strippedTweet;
        }

        public static void DetermineContent( Tweet tweet, Models.Challenge challenge )
        {
            Regex linkParser = new Regex( @"\b(?:https?://|www\.)\S+\b", RegexOptions.Compiled | RegexOptions.IgnoreCase );
            // Is image?
            if ( !( tweet.Media.FirstOrDefault() == null ) && challenge.Type.ToLower() == "image" )
            {
                SendResponse( tweet, challenge );
            }
            // Is URL?
            else if (linkParser.IsMatch(StripContent( tweet )) == true)
            {
                Regex youtubeTest = new Regex( "(https?://)?(www\\.)?(yotu\\.be/|youtube\\.com/)?((.+/)?(watch(\\?v=|.+&v=))?(v=)?)([\\w_-]{11})(&.+)?" );
                Regex instagramTest = new Regex( @"http://instagr\.?am(?:\.com)?/\S*" );
            }
            // Is text response
            else {
                string strippedTweet = StripContent( tweet );
            }
        }

        private static void SendResponse( Tweet tweet, Challenge challenge )
        {
            throw new NotImplementedException();
        }

        public static void TweetUser( string username, string message )
        {
            throw new NotImplementedException();
        }
    }
}