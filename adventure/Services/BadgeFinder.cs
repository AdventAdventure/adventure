using Adventure.Models;
using Adventure.Services;
using System.Linq;

namespace Adventure.Services
{
    public class BadgeFinder
    {
        private readonly AdventureContext _adventureContext;
        public BadgeFinder(AdventureContext adventureContext)
        {
            _adventureContext = adventureContext;
        }

        public void VerifyBadges(int userId)
        {
            VerifyBadgeFirstParticipation(_adventureContext, userId);
            VerifyBadgeFirstByType(_adventureContext, userId, BadgeCodes.Audio);
            VerifyBadgeFirstByType(_adventureContext, userId, BadgeCodes.Video);
            VerifyBadgeFirstByType(_adventureContext, userId, BadgeCodes.Image);
            _adventureContext.SaveChanges();
        }

        private static void VerifyBadgeFirstParticipation(AdventureContext context, int userId)
        {
            var badgeCount = (from b in context.Badges
                from ub in context.UserBadges.Where(x => x.UserId == userId)
                where b.Code == BadgeCodes.FirstParticipation.ToString()
                select b);

            if (context.UserChallenges.Count(x => x.UserId == userId && x.IsComplete == true) == 1 && !badgeCount.Any())
            {
                var badge = context.Badges.First(x => x.Code == BadgeCodes.FirstParticipation.ToString());

                if (badge != null)
                {
                    context.UserBadges.Add(new UserBadge
                                           {
                                               BadgeId = badge.BadgeId,
                                               UserId = userId
                                           });
                    string twitterUserName = context.Users.FirstOrDefault( x => x.UserId == userId ).UserName;
                    string badgeName = context.Badges.FirstOrDefault( b => b.BadgeId == badge.BadgeId ).Name;
                    string status = string.Format( "@{0} You've just earnt the {1} badge at @AdventHunters. Why not check it out? http://adventure-1.apphb.com/#/badges", twitterUserName, badgeName );
                    TwitterResponder.SendTweet( twitterUserName, status );
                }
            }
            
        }

        public void VerifyBadgeFirstByType(AdventureContext context, int userId, BadgeCodes code)
        {
            var result = (from r in context.UserChallenges
                          from c in context.Challenges.Where(x => x.ChallengeId == r.ChallengeId)
                where r.UserId == userId
                where c.Type == code.ToString()
                select r);

            var badgeCount = (from ub in context.UserBadges
                from b in context.Badges.Where(x => x.BadgeId == ub.BadgeId)
                where ub.UserId == userId
                where b.Code == code.ToString()
                select b);

            if (result.Count() == 1 && !badgeCount.Any())
            {
                var badge = context.Badges.First(x => x.Code == code.ToString());

                if (badge != null)
                {
                    context.UserBadges.Add(new UserBadge
                                           {
                                               BadgeId = badge.BadgeId,
                                               UserId = userId
                                           });
                }
            }
        }

        private void VerifyBadgeStreak(AdventureContext context, int userId)
        {
            int[] previousDays = (from uc in context.UserChallenges.Where(x => x.UserId == userId && x.IsComplete == true)
                from c in context.Challenges
                select c.ChallengeNumber)
                .ToArray();
            int streak = GetConsecutiveCount(previousDays);
            for (int i = 0; i <= 24;)
            {
                if (streak >= i)
                {
                    var streakBadge = (from ub in context.UserBadges
                        from b in context.Badges.Where(x => x.BadgeId == ub.BadgeId)
                        where ub.UserId == userId
                        where b.Code == "Streak" + i
                        select b);
                    if (streakBadge.Any())
                    {
                        var badge = context.Badges.First(x => x.Code == "Streak" + i.ToString());

                        context.UserBadges.Add(new UserBadge
                                               {
                                                   BadgeId = badge.BadgeId,
                                                   UserId = userId
                                               });

                    }

                }
                else
                {
                    break;
                }

                if (i <= 6)
                {
                    i += 3;
                }
                else
                {
                    i += 6;
                }
            }
        }

        public static int GetConsecutiveCount(int[] a)
        {
            int r = 1, t = 1;
            for (int i = 0; i < a.Length; i++)
            {
                bool c;
                if (i + 1 < a.Length)
                {
                    if (a[i + 1] - a[i] == 1)
                    {
                        t++;
                        c = true;
                    }
                    else
                    {
                        c = false;
                    }
                }
                else
                {
                    c = false;
                }
                if (t > r && !c)
                {
                    r = t;
                    t = 1;
                }
            }
            return r;
        }
    }
}