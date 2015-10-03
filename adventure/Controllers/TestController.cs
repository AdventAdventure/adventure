using Adventure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Adventure.Controllers
{
    public class TestController : ApiController
    {
        public HttpResponseMessage Get()
        {
            return null;
        }

        public void VerifyBadges(int userId)
        {
            using (var context = new AdventureContext())
            {


                //context.SaveChanges();
            }
        }

        public void VerifyBadgeFirstParticipation(AdventureContext context, int userId)
        {
            if (context.Responses.Count(x => x.UserId == userId) == 1)
            {
                /*var badge = context.Badges.First(x => x.Code == BadgeCodes.FirstParticipation);

                if (badge != null)
                {
                    context.UserBadges.Add(new UserBadge
                        {
                            BadgeId = badge.BadgeId,
                            UserId = userId
                        });
                }*/
            }
        }
    }
}
