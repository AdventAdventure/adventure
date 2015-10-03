using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text.RegularExpressions;


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
                //Not a submission
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
                if ( response == null && TwitterMessage.TimeStamp.Date <= DateTime.Now.Date )
                {
                    NewResponse( TwitterMessage, challenge );
                    DetermineContent();
                }
                else
                {
                    //Already submitted or submitted out of date bound
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
        // Change our account
        TweetUser( "Hey! That submission doesn't make any sense to us. Reply @adventiswhat if you think it should." );
    }

    public static void DetermineContent()
    {

    }

    public static void TweetUser( string message )
    {

    }
}
}