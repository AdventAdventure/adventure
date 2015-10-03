using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text.RegularExpressions;


namespace Adventure.Services
{
    public static class TweetParser
    {
        public static void main()
        {
            var TwitterMessage = new LinqToTwitter.Status();
            if ( !( TwitterMessage.RetweetedStatus == null ) | ( TwitterMessage.PossiblySensitive = false ) )
            {
                var twitterUser = TwitterMessage.User.UserIDResponse;
                var hashtags = ParseHashtags( TwitterMessage.Text );
                if ( hashtags.Any( h => !h.Value.Contains( "#submit" ) ) )
                {
                    return;
                }
                int i = 0;
                List<int> submitted = new List<int>();
                foreach ( Match m in hashtags )
                {
                    int day = int.Parse( Regex.Split( m.Value, "(#adventadventureday)((?:[0-9]*))" )[1] );
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
                        NewUser( twitterUser );
                    }
                    var response = adventureContext.Responses
                        .Where( r => r.ChallengeId == challenge.ChallengeId & r.UserId == int.Parse( twitterUser ) )
                        .FirstOrDefault();
                    if ( response == null && TwitterMessage.CreatedAt.Date <= DateTime.Now.Date )
                    {
                        NewResponse( TwitterMessage, challenge.ChallengeId );
                    }
                    else
                    {
                        i++;
                    }

                }
            }
        }

        public static Models.User NewUser( string twitterUser )
        {
            var newUser = new Models.User();
            var adventureContext = new AdventureContext();
            adventureContext.Users.Add( newUser );
            return newUser;
        }

        public static Models.Response NewResponse( LinqToTwitter.Status twitterMessage, int challengeId )
        {
            var response = new Models.Response();
            return response;
        }

        public static Match[] ParseHashtags( string s )
        {
            Match[] hashtags = Regex.Matches( s, "(#)((?:[A-Za-z0-9-_]*))" )
                       .Cast<Match>()
                       .ToArray();
            return hashtags;
        }

        public static void TweetUnknown( string twitterUser )
        {

        }
    }
}