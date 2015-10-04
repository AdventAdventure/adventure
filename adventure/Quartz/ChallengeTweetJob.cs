using Common.Logging;
using Quartz;
using System;

using System.Linq;
using Adventure.Services;

namespace Adventure.Quartz
{
    class ChallengeTweetJob : IJob
    {

        /// <summary> 
        /// Empty constructor for job initilization
        /// <para>
        /// Quartz requires a public empty constructor so that the
        /// scheduler can instantiate the class whenever it needs.
        /// </para>
        /// </summary>
        public ChallengeTweetJob()
        {

        }

        public void Execute( IJobExecutionContext context )
        {
            if (DateTime.Now.Month == 10 ) // Change this
            {
                var tweetStatus = new AdventureContext().Challenges.FirstOrDefault( t => t.ChallengeNumber == DateTime.UtcNow.Day ).InfoTweet;
                TwitterResponder.SendTweet( tweetStatus );

            }
        }
    }
}