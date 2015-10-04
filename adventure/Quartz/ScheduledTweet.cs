using System;
using System.Collections.Specialized;
using Quartz;
using Quartz.Impl;

namespace Adventure.Quartz
{
    public class ScheduledTweet : IScheduledJob
    {
        public void Run()
        {
            // Get an instance of the Quartz.Net scheduler
            var scheduler = GetScheduler();


            // Start the scheduler if its in standby
            if ( !scheduler.IsStarted )
            {
                scheduler.Start();
            }

            // Define the Job to be scheduled
            var job = JobBuilder.Create<ChallengeTweetJob>()
                .WithIdentity( "ScheduleTweet", "ScheduledTweets" )
                .RequestRecovery()
                .Build();

            // Associate a trigger with the Job
            var trigger = ( ICronTrigger ) TriggerBuilder.Create()
                .WithIdentity( "ScheduleTweet", "ScheduledTweets" )
                .WithCronSchedule( "0 0 9 1 / 1 * ? *" )
                .StartAt( DateTime.UtcNow )
                .WithPriority( 1 )
                .Build();

            if ( scheduler.CheckExists( new JobKey( "ScheduleTweet", "ScheduledTweets" ) ) )
            {
                scheduler.DeleteJob( new JobKey( "ScheduleTweet", "ScheduledTweets" ) );
            }

            var schedule = scheduler.ScheduleJob( job, trigger );
            Console.WriteLine( "Job '{0}' scheduled for '{1}'", "ScheduleTweet", schedule.ToString( "r" ) );
        }

        // Get an instance of the Quartz.Net scheduler
        private static IScheduler GetScheduler()
        {
            try
            {
                var properties = new NameValueCollection();
                properties["quartz.scheduler.instanceName"] = "ServerScheduler";

                // set remoting expoter
                properties["quartz.scheduler.proxy"] = "true";
                properties["quartz.scheduler.proxy.address"] = string.Format( "tcp://{0}:{1}/{2}", "localhost", "555",
                                                                             "QuartzScheduler" );

                // Get a reference to the scheduler
                var sf = new StdSchedulerFactory( properties );

                return sf.GetScheduler();

            }
            catch ( Exception ex )
            {
                Console.WriteLine( "Scheduler not available: '{0}'", ex.Message );
                throw;
            }
        }
    }
}